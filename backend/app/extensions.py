import os
import json
import firebase_admin
from firebase_admin import auth, credentials
from flask_sqlalchemy import SQLAlchemy
from flask_migrate import Migrate

firebase_creds_json = os.environ.get("FIREBASE_ADMIN_CREDENTIALS")
if not firebase_creds_json:
    raise ValueError("FIREBASE_ADMIN_CREDENTIALS environment variable not set.")

firebase_creds_dict = json.loads(firebase_creds_json)

cred = credentials.Certificate(firebase_creds_dict)
firebase_admin.initialize_app(cred)

db = SQLAlchemy()
migrate = Migrate()
