# 🌟 FluxGuard: Advanced Remote System Control via Telegram Bot 🌟  

FluxGuard is an 🛠️ **innovative Telegram bot** that provides 📡 **full remote control** over your system. Designed with a focus on 🔐 **security**, ⚡ **efficiency**, and 🖥️ **user convenience**, it allows you to manage your computer from anywhere using Telegram!

---

## 🚀 **Key Features**  
Based on the existing code, FluxGuard enables the following capabilities:  

### 💻 **System Control**  
- **Power Management:** 📴 Shutdown, 🔄 Restart, or 💤 Put the system to sleep remotely.  
- **Screenshot Capture:** 📸 Take screenshots of the desktop or specific applications.  
- **Process Management:** 📋 View running applications, capture their screenshots, or ❌ terminate them.  
- **File Exploration:** 📂 Browse drives and folders, view file details, 📤 send files to Telegram, or 🗑️ delete them.  

### ⚙️ **Monitoring & Reports**  
- **System Resources:** 📊 Receive text-based reports on CPU, RAM, GPU, and network status.  
- **Uptime Tracking:** ⏱️ View system uptime.  

### 🔐 **Security & Customization**  
- **Multi-language Support:** 🌍 Choose between English and Persian.  
- **Secure Communication:** 🔒 Uses Telegram's secure HTTPS API for data exchange.  
- **Configurable Settings:** 🛠️ Modify bot token, chat ID, language, and auto-start settings via JSON files.  

### 🖥️ **User Interfaces**  
- **Telegram Bot:** 🤖 Interactive menus with inline and reply buttons for easy management.  
- **Command Line Interface (CLI):** 🖱️ Enables manual bot control via terminal.  
- **Graphical User Interface (GUI) (Upcoming):** 🖼️ A visual interface for enhanced control.  

---

## 💡 **Why Choose FluxGuard?**  
✅ **🌐 Remote Access:** Manage your system from anywhere with an internet connection.  
✅ **🔐 High Security:** Secure, encrypted communication via Telegram's platform.  
✅ **🤩 User-Friendly Design:** Simple, intuitive, and easy to use.  

---

## 🛠️ **Installation Guide**  

### 1. **Prerequisites**  
- **Install .NET Runtime:** [Download here](https://dotnet.microsoft.com/download)  
- **Obtain Telegram Bot Token:** Create a bot via [BotFather](https://core.telegram.org/bots#how-do-i-create-a-bot)  
- **Get Your Chat ID:** Use bots like [GetIDBot](https://t.me/getidbot)  

### 2. **Configuration**  
- **Edit `config.json`:**  
  ```json  
  {  
    "telegram_bot_token": "YOUR_BOT_TOKEN",  
    "user_chat_id": "YOUR_CHAT_ID"  
  }  
  ```  
- **Edit `setting.json` (Optional):**  
  ```json  
  {  
    "telegram_bot_language": "en",  
    "automatic_start": false  
  }  
  ```  

### 3. **Run the Application**  
- **Build the Project:**  
  ```bash  
  dotnet build  
  ```  
- **Start the Program:**  
  - Run `GUI.exe` for GUI-based interaction.  
  - Use the CLI for manual bot control.  

---

## 📖 **How to Use**  

### 🤖 **Using the Telegram Bot**  

1. **Start the Bot:**  
   - Send `/start` to the bot.  
   - Choose a language (English or Persian).  

2. **Main Menu Options:**  
   - **Status:** View system status.  
   - **Power Management:** Shutdown, restart, or sleep.  
   - **Processes:** View and manage running applications.  
   - **File Transfer (Upcoming).**  
   - **File Management:** Browse and manage files.  
   - **Security & General Settings (Future Features).**  

3. **Core Commands:**  
   - **Power Management:**  
     - View a screenshot before shutting down, restarting, or putting the system to sleep.  
   - **Process Management:**  
     - View running applications, capture their screenshots, or terminate them.  
   - **File Management:**  
     - Browse drives, view file details, delete, or send files to Telegram.  
   - **System Status:**  
     - View system resource usage and uptime.  

4. **Navigation:**  
   - Use reply buttons for quick selections.  
   - Inline buttons for nested options (e.g., process management).  

### 🖱️ **Using CLI**  

**Available Commands:**  

| **Command**       | **Description**                          |  
|-------------------|------------------------------------------|  
| `flux-s`         | Start the Telegram bot.                  |  
| `flux-x`         | Stop the bot.                            |  
| `status`         | Display bot status.                      |  
| `logs [hours]`   | View logs for the past X hours.          |  
| `config`         | Edit bot token and chat ID.              |  
| `settings`       | Change language and auto-start options.  |  
| `help`           | Show available commands.                 |  
| `exit`           | Exit the program.                        |  

**Examples:**  
- Start the bot: `flux-s`  
- View logs from the last 2 hours: `logs 2`  
- Change language to Persian: `settings` → Select "Telegram Bot Language" → Enter `fa`  

---

## 🗂️ **Project Structure**  

- **`FluxGuard.Core`:** Core logic, bot, CLI, data handling, and services.  
- **`FluxGuard.GUI`:** GUI components (such as screenshot capture).  
- **`lan/`:** Language files (`en.json`, `fa.json`).  
- **`logs/`:** Log files.  

---

## 🔗 Dependencies & Architecture  

### FluxGuard follows a modular structure where core components interact through defined dependencies.  

![fluxguard_graph_clean](https://github.com/user-attachments/assets/e190ea67-a48b-492e-a1a1-9bce08c100bc)

---

## 🔮 **[Future Plans](https://github.com/tahadashti-gd/FluxGuard/issues)** 

### Visit issues page.
---

## 🙌 **Contributions & Support**  

💬 **Your feedback matters!**  
Help improve FluxGuard by reporting issues, suggesting new features, or contributing to the development! 🚀  

---

## 📜 **License**  

📝 This project is licensed under **[GPL v3](https://www.gnu.org/licenses/gpl-3.0.en.html)**.  

---

## [Buy me a cafe](https://daramet.com/TahaDashti)

