from django.db import models
from django.core.validators import MinValueValidator, MaxValueValidator
from django.contrib.auth.models import AbstractUser
from django.contrib.auth.models import BaseUserManager

class UserManager(BaseUserManager):
    def create_user(self, email, full_name, password=None, **extra_fields):
        if not email:
            raise ValueError('Пользователь должен иметь email')
        email = self.normalize_email(email)
        user = self.model(email=email, full_name=full_name, **extra_fields)
        user.set_password(password)
        user.save(using=self._db)
        return user

    def create_superuser(self, email, full_name, password=None, **extra_fields):
        extra_fields.setdefault('is_staff', True)
        extra_fields.setdefault('is_superuser', True)
        extra_fields.setdefault('is_active', True)

        if extra_fields.get('is_staff') is not True:
            raise ValueError('Суперпользователь должен иметь is_staff=True.')
        if extra_fields.get('is_superuser') is not True:
            raise ValueError('Суперпользователь должен иметь is_superuser=True.')

        return self.create_user(email, full_name, password, **extra_fields)

class User(AbstractUser):
    username = None
    email = models.EmailField(unique=True)
    full_name = models.CharField(max_length=255)
    phone = models.CharField(max_length=20, blank=True, null=True)
    photo_url = models.TextField(blank=True, null=True)
    rating = models.DecimalField(
        max_digits=3,
        decimal_places=2,
        default=0.00,
        validators=[MinValueValidator(0.00), MaxValueValidator(5.00)]
    )
    city = models.CharField(max_length=100, blank=True, null=True)
    registration_date = models.DateTimeField(auto_now_add=True)

    USERNAME_FIELD = 'email'
    REQUIRED_FIELDS = ['full_name']

    objects = UserManager()  

    def __str__(self):
        return self.full_name or self.email

    class Meta:
        db_table = 'users'
        verbose_name = 'Пользователь'
        verbose_name_plural = 'Пользователи'


class Category(models.Model):
    name = models.CharField(max_length=100, unique=True)

    def __str__(self):
        return self.name

    class Meta:
        db_table = 'categories'
        verbose_name = 'Категория'
        verbose_name_plural = 'Категории'


class Skill(models.Model):
    LEVEL_CHOICES = [
        ('новичок', 'Новичок'),
        ('средний', 'Средний'),
        ('эксперт', 'Эксперт'),
    ]

    name = models.CharField(max_length=255)
    description = models.TextField(blank=True, null=True)
    level = models.CharField(max_length=20, choices=LEVEL_CHOICES)
    category = models.ForeignKey(
        Category,
        on_delete=models.CASCADE,
        related_name='skills'
    )

    def __str__(self):
        return f"{self.name} ({self.get_level_display()})"

    class Meta:
        db_table = 'skills'
        verbose_name = 'Навык'
        verbose_name_plural = 'Навыки'


class UserSkill(models.Model):
    user = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='user_skills'
    )
    skill = models.ForeignKey(
        Skill,
        on_delete=models.CASCADE,
        related_name='skill_users'
    )

    class Meta:
        db_table = 'user_skills'
        unique_together = ('user', 'skill')
        verbose_name = 'Навык пользователя'
        verbose_name_plural = 'Навыки пользователей'

    def __str__(self):
        return f"{self.user.full_name} - {self.skill.name}"


class Proposal(models.Model):
    FORMAT_CHOICES = [
        ('онлайн', 'Онлайн'),
        ('офлайн', 'Офлайн'),
    ]

    user = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='proposals'
    )
    skill_offered = models.ForeignKey(
        Skill,
        on_delete=models.CASCADE,
        related_name='proposals_as_offered',
        verbose_name="Навык, который предлагает"
    )
    skill_wanted = models.ForeignKey(
        Skill,
        on_delete=models.CASCADE,
        related_name='proposals_as_wanted',
        verbose_name="Навык, который хочет получить"
    )
    description = models.TextField(blank=True, null=True)
    format = models.CharField(
        max_length=20,
        choices=FORMAT_CHOICES,
        default='онлайн'
    )
    deadlines = models.TextField(blank=True, null=True)
    created_at = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        return f"{self.user.full_name}: {self.skill_offered.name} → {self.skill_wanted.name}"

    class Meta:
        db_table = 'proposals'
        verbose_name = 'Предложение обмена'
        verbose_name_plural = 'Предложения обмена'


class Request(models.Model):
    user = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='requests'
    )
    skill_wanted = models.ForeignKey(
        Skill,
        on_delete=models.CASCADE,
        related_name='requests_as_wanted',
        verbose_name="Хочет научиться"
    )
    skill_offered = models.ForeignKey(
        Skill,
        on_delete=models.CASCADE,
        related_name='requests_as_offered',
        verbose_name="Готов отдать взамен"
    )
    description = models.TextField(blank=True, null=True)
    created_at = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        return f"Запрос: {self.user.full_name} хочет {self.skill_wanted.name}, предлагая {self.skill_offered.name}"

    class Meta:
        db_table = 'requests'
        verbose_name = 'Запрос на обучение'
        verbose_name_plural = 'Запросы на обучение'


class Exchange(models.Model):
    STATUS_CHOICES = [
        ('активен', 'Активен'),
        ('завершён', 'Завершён'),
        ('отклонён', 'Отклонён'),
    ]
    FORMAT_CHOICES = [
        ('онлайн', 'Онлайн'),
        ('офлайн', 'Офлайн'),
    ]

    user1 = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='exchanges_as_user1'
    )
    user2 = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='exchanges_as_user2'
    )
    proposal = models.ForeignKey(
        Proposal,
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='exchanges'
    )
    request = models.ForeignKey(
        Request,
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='exchanges'
    )
    status = models.CharField(
        max_length=20,
        choices=STATUS_CHOICES,
        default='активен'
    )
    start_date = models.DateTimeField(null=True, blank=True)
    end_date = models.DateTimeField(null=True, blank=True)
    format = models.CharField(
        max_length=20,
        choices=FORMAT_CHOICES,
        default='онлайн'
    )
    location = models.TextField(blank=True, null=True)
    notes = models.TextField(blank=True, null=True)
    created_at = models.DateTimeField(auto_now_add=True)

    class Meta:
        db_table = 'exchanges'
        verbose_name = 'Обмен'
        verbose_name_plural = 'Обмены'
        constraints = [
            models.CheckConstraint(
                check=~models.Q(user1=models.F('user2')),
                name='check_user1_not_equal_user2'
            )
        ]

    def __str__(self):
        return f"Обмен между {self.user1.full_name} и {self.user2.full_name}"


class Review(models.Model):
    reviewer = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='reviews_given'
    )
    reviewed_user = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='reviews_received'
    )
    exchange = models.ForeignKey(
        Exchange,
        on_delete=models.CASCADE,
        related_name='reviews'
    )
    text = models.TextField(blank=True, null=True)
    rating = models.SmallIntegerField(
        validators=[MinValueValidator(1), MaxValueValidator(5)]
    )
    created_at = models.DateTimeField(auto_now_add=True)

    class Meta:
        db_table = 'reviews'
        verbose_name = 'Отзыв'
        verbose_name_plural = 'Отзывы'
        constraints = [
            models.CheckConstraint(
                check=~models.Q(reviewer=models.F('reviewed_user')),
                name='check_reviewer_not_self'
            )
        ]

    def __str__(self):
        return f"Отзыв от {self.reviewer.full_name} для {self.reviewed_user.full_name} ({self.rating}★)"


class Message(models.Model):
    exchange = models.ForeignKey(
        Exchange,
        on_delete=models.CASCADE,
        related_name='messages'
    )
    sender = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='sent_messages'
    )
    receiver = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='received_messages'
    )
    text = models.TextField()
    sent_at = models.DateTimeField(auto_now_add=True)
    is_read = models.BooleanField(default=False)

    def __str__(self):
        return f"Сообщение от {self.sender.full_name} → {self.receiver.full_name}"

    class Meta:
        db_table = 'messages'
        verbose_name = 'Сообщение'
        verbose_name_plural = 'Сообщения'


class Event(models.Model):
    title = models.CharField(max_length=255)
    description = models.TextField(blank=True, null=True)
    organizer = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='organized_events'
    )
    event_date = models.DateField()
    event_time = models.TimeField(null=True, blank=True)
    location = models.TextField(blank=True, null=True)
    max_participants = models.IntegerField(default=10)
    category = models.ForeignKey(
        Category,
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='events'
    )
    created_at = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        return self.title

    class Meta:
        db_table = 'events'
        verbose_name = 'Мероприятие'
        verbose_name_plural = 'Мероприятия'


class EventParticipation(models.Model):
    STATUS_CHOICES = [
        ('подтверждён', 'Подтверждён'),
        ('ожидание', 'Ожидание'),
        ('отказ', 'Отказ'),
    ]

    user = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='event_participations'
    )
    event = models.ForeignKey(
        Event,
        on_delete=models.CASCADE,
        related_name='participations'
    )
    status = models.CharField(
        max_length=20,
        choices=STATUS_CHOICES,
        default='ожидание'
    )
    registered_at = models.DateTimeField(auto_now_add=True)

    class Meta:
        db_table = 'event_participations'
        unique_together = ('user', 'event')
        verbose_name = 'Участие в мероприятии'
        verbose_name_plural = 'Участия в мероприятиях'

    def __str__(self):
        return f"{self.user.full_name} - {self.event.title} ({self.get_status_display()})"


class Point(models.Model):
    user = models.OneToOneField(
        User,
        on_delete=models.CASCADE,
        related_name='points'
    )
    balance = models.IntegerField(
        default=0,
        validators=[MinValueValidator(0)]
    )

    def __str__(self):
        return f"{self.user.full_name}: {self.balance} очков"

    class Meta:
        db_table = 'points'
        verbose_name = 'Очки пользователя'
        verbose_name_plural = 'Очки пользователей'


class PointTransaction(models.Model):
    REASON_CHOICES = [
        ('за обучение', 'За обучение'),
        ('за отзыв', 'За отзыв'),
        ('покупка премиума', 'Покупка премиума'),
        ('ручная корректировка', 'Ручная корректировка'),
    ]

    user = models.ForeignKey(
        User,
        on_delete=models.CASCADE,
        related_name='point_transactions'
    )
    amount = models.IntegerField()
    reason = models.CharField(max_length=50, choices=REASON_CHOICES)
    exchange = models.ForeignKey(
        Exchange,
        on_delete=models.SET_NULL,
        null=True,
        blank=True,
        related_name='point_transactions'
    )
    created_at = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        sign = "+" if self.amount >= 0 else ""
        return f"{sign}{self.amount} очков для {self.user.full_name} ({self.get_reason_display()})"

    class Meta:
        db_table = 'point_transactions'
        verbose_name = 'Транзакция очков'
        verbose_name_plural = 'Транзакции очков'