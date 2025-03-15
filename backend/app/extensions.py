from flask_sqlalchemy import SQLAlchemy
from flask_migrate import Migrate
import firebase_admin
from firebase_admin import auth, credentials

cred = credentials.Certificate("firebase_admin.json")
firebase_admin.initialize_app(cred)

db = SQLAlchemy()
migrate = Migrate()
