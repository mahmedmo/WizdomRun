from app.extensions import db
from sqlalchemy import Enum, Numeric, CheckConstraint, func
from sqlalchemy.orm import relationship

class User(db.Model):
    __tablename__ = 'users'

    userID = db.Column(db.Integer, primary_key=True)
    screenName = db.Column(db.String(31), unique=True, nullable=False)
    createdAt = db.Column(db.TIMESTAMP, server_default=func.now())

    characters = relationship("PlayerCharacter", backref="user", cascade="all, delete-orphan")
    campaigns = relationship("Campaign", backref="user", cascade="all, delete-orphan")

class PlayerCharacter(db.Model):
    __tablename__ = 'player_character'

    characterID = db.Column(db.Integer, primary_key=True)
    userID = db.Column(db.Integer, db.ForeignKey('users.userID', ondelete="CASCADE"), nullable=False)
    modelID = db.Column(db.Integer, nullable=False)
    hairID = db.Column(db.Integer)
    robeID = db.Column(db.Integer)
    bootID = db.Column(db.Integer)

class Campaign(db.Model):
    __tablename__ = 'campaign'

    campaignID = db.Column(db.Integer, primary_key=True)
    lastUpdated = db.Column(db.TIMESTAMP, server_default=func.now())
    userID = db.Column(db.Integer, db.ForeignKey('users.userID', ondelete="CASCADE"), nullable=False)
    title = db.Column(db.String(127), unique=True, nullable=False)
    campaignLength = db.Column(Enum('quest', 'odyssey', 'saga', name='length_type'), nullable=False)
    currLevel = db.Column(db.Integer, nullable=False)
    remainingTries = db.Column(db.Integer, nullable=False, default=2)

    questions = relationship("Question", backref="campaign", cascade="all, delete-orphan")
    player_stats = relationship("PlayerStats", backref="campaign", uselist=False, cascade="all, delete-orphan")
    achievements = relationship("Achievement", backref="campaign", cascade="all, delete-orphan")

class Question(db.Model):
    __tablename__ = 'questions'

    questionID = db.Column(db.Integer, primary_key=True)
    campaignID = db.Column(db.Integer, db.ForeignKey('campaign.campaignID', ondelete="CASCADE"), nullable=False)
    difficulty = db.Column(Enum('easy', 'medium', 'hard', name='question_difficulty'), nullable=False)
    questionStr = db.Column(db.Text, nullable=False)
    gotCorrect = db.Column(db.Boolean, nullable=False, default=False)
    wrongAttempts = db.Column(db.Integer, nullable=False, default=0)

    answers = relationship("Answer", backref="question", cascade="all, delete-orphan")

class Answer(db.Model):
    __tablename__ = 'answers'

    answerID = db.Column(db.Integer, primary_key=True)
    questionID = db.Column(db.Integer, db.ForeignKey('questions.questionID', ondelete="CASCADE"), nullable=False)
    answerStr = db.Column(db.Text, nullable=False)
    isCorrect = db.Column(db.Boolean, nullable=False, default=False)

class PlayerStats(db.Model):
    __tablename__ = 'player_stats'

    campaignID = db.Column(db.Integer, db.ForeignKey('campaign.campaignID', ondelete="CASCADE"), primary_key=True)
    attack = db.Column(Numeric(2, 1), nullable=False)
    hp = db.Column(db.Integer, nullable=False, default=100)
    mana = db.Column(db.Integer, nullable=False, default=0)
    affinity = db.Column(Enum('fire', 'earth', 'water', 'air', name='playerClass'))

    __table_args__ = (
        CheckConstraint('attack >= 1 AND attack <= 5', name='player_stats_attack_check'),
    )

class Spell(db.Model):
    __tablename__ = 'spells'

    spellID = db.Column(db.Integer, primary_key=True)
    spellName = db.Column(db.String(31), nullable=False)
    description = db.Column(db.Text)
    spellElement = db.Column(Enum('fire', 'earth', 'water', 'air', name='playerClass'), nullable=False)

class PlayerSpells(db.Model):
    __tablename__ = 'player_spells'

    playerspellID = db.Column(db.Integer, primary_key=True)
    playerID = db.Column(db.Integer, db.ForeignKey('player_stats.campaignID'), nullable=False)
    spellID = db.Column(db.Integer, db.ForeignKey('spells.spellID'), nullable=False)

class Achievement(db.Model):
    __tablename__ = 'achievements'

    achievementID = db.Column(db.Integer, primary_key=True)
    campaignID = db.Column(db.Integer, db.ForeignKey('campaign.campaignID', ondelete="CASCADE"), nullable=False)
    title = db.Column(db.String(63), nullable=False)
    description = db.Column(db.Text)


# test db connection
# from app.extensions import db
# from app.models import User, PlayerProgress

# # Create a new user
# new_user = User(user_id="user_123")
# db.session.add(new_user)
# db.session.commit()

# User.query.all()

