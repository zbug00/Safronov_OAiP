from django.contrib import admin
from .models import (
    User, Category, Skill, UserSkill, Proposal, Request,
    Exchange, Review, Message, Event, EventParticipation,
    Point, PointTransaction
)

@admin.register(User)
class UserAdmin(admin.ModelAdmin):
    list_display = ['email', 'full_name', 'rating', 'city', 'registration_date']
    search_fields = ['email', 'full_name', 'city']
    list_filter = ['city', 'rating']
    ordering = ['-registration_date']

@admin.register(Category)
class CategoryAdmin(admin.ModelAdmin):
    list_display = ['name']
    search_fields = ['name']

@admin.register(Skill)
class SkillAdmin(admin.ModelAdmin):
    list_display = ['name', 'level', 'category']
    list_filter = ['category', 'level']
    search_fields = ['name', 'description']

@admin.register(UserSkill)
class UserSkillAdmin(admin.ModelAdmin):
    list_display = ['user', 'skill']
    list_filter = ['skill__category']
    search_fields = ['user__email', 'skill__name']

@admin.register(Proposal)
class ProposalAdmin(admin.ModelAdmin):
    list_display = ['user', 'skill_offered', 'skill_wanted', 'format', 'created_at']
    list_filter = ['format', 'created_at']
    search_fields = ['user__email', 'description']
    raw_id_fields = ['user', 'skill_offered', 'skill_wanted']  # для удобства при большом количестве записей

@admin.register(Request)
class RequestAdmin(admin.ModelAdmin):
    list_display = ['user', 'skill_wanted', 'skill_offered', 'created_at']
    list_filter = ['created_at']
    search_fields = ['user__email', 'description']
    raw_id_fields = ['user', 'skill_wanted', 'skill_offered']

@admin.register(Exchange)
class ExchangeAdmin(admin.ModelAdmin):
    list_display = ['user1', 'user2', 'status', 'format', 'start_date', 'end_date']
    list_filter = ['status', 'format', 'start_date']
    search_fields = ['user1__email', 'user2__email']
    raw_id_fields = ['user1', 'user2', 'proposal', 'request']

@admin.register(Review)
class ReviewAdmin(admin.ModelAdmin):
    list_display = ['reviewer', 'reviewed_user', 'rating', 'exchange', 'created_at']
    list_filter = ['rating', 'created_at']
    search_fields = ['reviewer__email', 'reviewed_user__email', 'text']

@admin.register(Message)
class MessageAdmin(admin.ModelAdmin):
    list_display = ['sender', 'receiver', 'exchange', 'sent_at', 'is_read']
    list_filter = ['is_read', 'sent_at']
    search_fields = ['sender__email', 'receiver__email', 'text']
    raw_id_fields = ['exchange', 'sender', 'receiver']

@admin.register(Event)
class EventAdmin(admin.ModelAdmin):
    list_display = ['title', 'organizer', 'event_date', 'category', 'max_participants']
    list_filter = ['event_date', 'category']
    search_fields = ['title', 'description', 'organizer__email']
    raw_id_fields = ['organizer', 'category']

@admin.register(EventParticipation)
class EventParticipationAdmin(admin.ModelAdmin):
    list_display = ['user', 'event', 'status', 'registered_at']
    list_filter = ['status', 'registered_at']
    search_fields = ['user__email', 'event__title']

@admin.register(Point)
class PointAdmin(admin.ModelAdmin):
    list_display = ['user', 'balance']
    search_fields = ['user__email']

@admin.register(PointTransaction)
class PointTransactionAdmin(admin.ModelAdmin):
    list_display = ['user', 'amount', 'reason', 'exchange', 'created_at']
    list_filter = ['reason', 'created_at']
    search_fields = ['user__email']
    raw_id_fields = ['user', 'exchange']