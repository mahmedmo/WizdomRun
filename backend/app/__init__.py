from flask import Flask
from app.config import Config
from app.extensions import db, migrate
import logging
from app.routes.users import users_bp
from app.routes.campaigns import campaigns_bp
from app.routes.questions import questions_bp
from app.routes.stats import stats_bp
from app.routes.achievements import achievements_bp
from app.routes.characters import characters_bp

def create_app():
    app = Flask(__name__)
    app.config.from_object(Config)

    db.init_app(app)
    migrate.init_app(app, db)

    logging.basicConfig(level=logging.INFO)
    logger = logging.getLogger(__name__)

    app.register_blueprint(users_bp, url_prefix="/users")
    app.register_blueprint(campaigns_bp, url_prefix="/campaigns")
    app.register_blueprint(questions_bp, url_prefix="/questions")
    app.register_blueprint(stats_bp, url_prefix="/stats")
    app.register_blueprint(achievements_bp, url_prefix="/achievements")
    app.register_blueprint(characters_bp, url_prefix="/characters")
    return app

