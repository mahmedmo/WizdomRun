import pyrebase

firebase_config = {
    "apiKey": "AIzaSyAqkwtLQ0vIJ9lYLW9iqVIR7_HzrdXqN7c",
    "authDomain": "wizdomrun.firebaseapp.com",
    "databaseURL": "https://dummy.firebaseio.com",
    "storageBucket": "wizdomrun.appspot.com"
}

firebase = pyrebase.initialize_app(firebase_config)
auth = firebase.auth()

# Replace with a real test user
email = "email"
password = "password"

try:
    user = auth.sign_in_with_email_and_password(email, password)
    id_token = user['idToken']
    print(f"Firebase ID Token: {id_token}")
except Exception as e:
    print(f"Error: {e}")
