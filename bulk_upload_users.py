import json
import requests
import logging
import sys

API_URL = "http://localhost:5000/api/users"
JSON_PATH = "UserManagementAPI/sample-users.json"

logging.basicConfig(level=logging.INFO, format='%(asctime)s %(levelname)s %(message)s')

def debug_breakpoint():
    '''Set a breakpoint for debugging.'''
    if sys.gettrace() is not None:
        breakpoint()

def upload_users():
    try:
        with open(JSON_PATH, "r") as f:
            users = json.load(f)
    except Exception as e:
        logging.error(f"Failed to load users from {JSON_PATH}: {e}")
        debug_breakpoint()
        return

    for idx, user in enumerate(users, 1):
        try:
            resp = requests.post(API_URL, json=user)
            if resp.status_code not in (200, 201):
                logging.error(f"Failed to upload user #{idx} {user['email']}: {resp.status_code} {resp.text}")
                debug_breakpoint()
            else:
                logging.info(f"Uploaded user #{idx}: {user['email']} (Status: {resp.status_code})")
        except Exception as e:
            logging.error(f"Exception uploading user #{idx} {user['email']}: {e}")
            debug_breakpoint()

if __name__ == "__main__":
    upload_users()
