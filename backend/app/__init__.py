from flask import Flask
from .config import Config        
from .extensions import db, migrate
import logging
from .routes.users import users_bp
from .routes.campaigns import campaigns_bp
from .routes.questions import questions_bp
from .routes.stats import stats_bp
from .routes.achievements import achievements_bp
from .routes.characters import characters_bp
from .routes.auth import auth_bp

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
    app.register_blueprint(auth_bp, url_prefix="/auth")
    return app
