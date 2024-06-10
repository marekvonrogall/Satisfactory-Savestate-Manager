# Satisfactory-Savestate-Manager

## Purpose
This application is used to synchronize save states of the game "[Satisfactory](https://www.satisfactorygame.com/)" outside of the default Steam cloud, allowing synchronization between multiple users.
This feature comes in handy if you plan to play with friends and seek a simple solution for syncing the save state. It allows everyone to keep playing on a save state, even if the original session creator isn't available to host the world.

## Requirements

In order to use this application you need to create a google drive service account and import your credentials.
After creating and downloading a Public Key Certificate under your [service account](https://console.cloud.google.com/iam-admin/serviceaccounts/) details, you need to create a folder on your [Google Drive](https://drive.google.com/drive) and permit access to the e-mail associated with your service account.
The application will save all savestates in this folder.
Copy the folder ID of the folder and create the following new line in the Public-Key-Certificate file you just downloaded:

```json
"folder_id": "your_google_drive_folder_id"
```

Here's an example of the required JSON structure:

```json
{
  "type": "service_account",
  "project_id": "your_project_name",
  "private_key_id": "PK_ID_here",
  "private_key": "-----BEGIN PRIVATE KEY-----\n your_private_key \n-----END PRIVATE KEY-----\n",
  "client_email": "your_client_email",
  "client_id": "your_client_id",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/your_project_cert_url",
  "universe_domain": "googleapis.com",
  "folder_id": "your_google_drive_folder_id"
}
```

## Setup

Running the application for the first time requires the user to provide his Google Drive credentials. This is necessary in order to upload and download files from Google Drive.
Additionaly, the user will be prompted to enter a save state path. It is recommended to use the default path, which corresponds to the default Steam save state location for Satisfactory.

The default Steam save state location is: `C:\Users\USER_NAME\AppData\Local\FactoryGame\Saved\SaveGames\YOUR_STEAM_ID`

After the completion of the setup, the user can choose between a list of local save files and remote save files, which allows for easy uploading and downloading.
All files are sorted by the session name.

## Possible Errors

If your Google Drive credentials are invalid, the program may not start or function properly. Likewise, if you are not connected to the internet, the program may encounter the same issue.
As of now, the application has exclusively been tested with the Steam version of Satisfactory (Windows 11). You may encounter errors if attempting to use the application with the Epic Games version of Satisfactory without having Steam installed.
