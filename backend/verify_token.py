import base64
import json

token = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjNjI2MmYzZTk3NzIzOWMwMDUzY2ViODY0Yjc3NDBmZjMxZmNkY2MiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vd2l6ZG9tcnVuIiwiYXVkIjoid2l6ZG9tcnVuIiwiYXV0aF90aW1lIjoxNzQwOTU1NzI0LCJ1c2VyX2lkIjoiSm9EcUYwSWkzNmU5a3FTSndSZ05LQ21UclRmMSIsInN1YiI6IkpvRHFGMElpMzZlOWtxU0p3UmdOS0NtVHJUZjEiLCJpYXQiOjE3NDA5NTU3MjQsImV4cCI6MTc0MDk1OTMyNCwiZW1haWwiOiJjaGFybGllbGFuZy5za2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImNoYXJsaWVsYW5nLnNraUBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.acheho6bNue-l9XpvsB_N7z-DrXL_fEB4UP9sMnF-9tR--oSM737w45VcmjgOS3vz3DUh7C55ud7E7ihGaXqRLmu0TC5EnZJp8mjPRGE-eJ-LZZU63dWmVuATEtFr8fDasXY6hlLWoEhXqF4Iswhq15shPAG9VsNBHs0SAC-G1BUV8oG8XQn6uT2wQ9jN-2b10DRXl_LkKT_-wR8E45iUljiSSIUOymwp1LJSqsA00ZaqRU2pF3D3BinYpqixpxc7XRTp1G0I4z6ocKF7k-xGgBRadRbyW-6-EVe5Ifo1J7KdqtL4eODu68v8JiiRA4ByZw-GeFXsduhS9ci7_o17Q"  # Replace with your actual token

payload_b64 = token.split(".")[1]

missing_padding = len(payload_b64) % 4
if missing_padding:
    payload_b64 += "=" * (4 - missing_padding)

try:
    decoded_payload = base64.urlsafe_b64decode(payload_b64).decode("utf-8")
    payload_json = json.loads(decoded_payload)
    print(json.dumps(payload_json, indent=4))
except Exception as e:
    print(f"Error decoding token: {e}")
