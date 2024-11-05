# 🗄️ Backup and Restore Guide

StreamMaster includes a built-in backup and restore feature to help you safeguard your data and settings. With automated backups and a straightforward restore process, you can confidently recover your configurations if needed.

---

## Backups 📋

To enable automatic backups in StreamMaster, adjust the following settings:

- **Enable**: Enable backups
- **Versions**: Defines the number of backup versions to retain. Once this limit is reached, older backups are deleted to make room for new ones.
- **Interval**: Specifies how often (in hours) backups are created.

StreamMaster will now automatically back up data every `Interval` hours and maintain the latest backup `Versions` as specified.

---

## Restoring a Backup 🔄

If you need to restore a backup, you can do so by following these steps:

### Steps to Restore a Backup

1.  **Shut down the StreamMaster container**:

        - This step is essential to avoid data conflicts during the restore process.

1.  **Copy the backup file**:

        - Navigate to the `config/Backups` directory and locate the backup file you want to restore.
        - Copy this file to the `config/Restore` directory.

1.  **Start the container**:

        - Once the backup file is in the `config/Restore` folder, start the StreamMaster container.
        - The system will automatically detect the file and restore data from the backup.

### Important Considerations

- After completing the restore, StreamMaster will continue with its regular backup schedule.
- Ensure the `config/Restore` directory contains only the file you intend to restore. Any backup file here will be used for restoration.

---

By using these backup and restore features, you can protect your data and recover quickly from potential issues.

---

{% include-markdown "../includes/_footer.md" %}
