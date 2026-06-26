# Cybersecurity Awareness Assistant

## Student Information

**Name:** Zipho  
**Student Number:** ST10516063  
**Module:** PROG6221 Programming 2A  
**Assessment:** Part 3 / Final POE

## Project Description

The Cybersecurity Awareness Assistant is a C# WPF desktop application that teaches users how to stay safer online. The final application combines all Part 1 and Part 2 chatbot features with a persistent cybersecurity task assistant, reminders, a 12-question cybersecurity quiz, flexible NLP-style command recognition, and a timestamped activity log.

## Part 1 Features

- WAV voice greeting when the application starts
- Cybersecurity ASCII art
- Personalised name greeting
- Input validation and fallback messages
- Cybersecurity awareness responses

## Part 2 Features

- WPF graphical user interface
- Keyword recognition and randomised responses
- Follow-up conversation flow, including `tell me more`
- Sentiment detection for worried, curious, frustrated, and happy users
- Memory for the user's name and favourite cybersecurity topic
- Personalised cybersecurity tips

## Part 3 Features

### Task Assistant and Reminders

- Add tasks from the Task Assistant tab or through chatbot commands
- Store a title, description, optional reminder, completion status, and creation date
- Read saved tasks automatically when the application starts
- Mark a selected task as complete
- Delete a selected task
- Save every change immediately to `tasks.json`
- Interpret reminders such as `tomorrow`, `in 5 days`, `in 2 weeks`, and normal date text

### Cybersecurity Mini-Game

- 12 cybersecurity questions
- Multiple-choice and true/false formats
- One question displayed at a time
- Immediate correct or incorrect feedback
- Explanation after every answer
- Live score and final feedback message
- Restart option

### NLP Simulation

The application uses case-insensitive string matching and regular expressions to understand varied wording. Examples include:

- `Add a task to enable two-factor authentication`
- `Create a task for reviewing my privacy settings`
- `Remind me to update my password in 5 days`
- `Start the quiz`
- `Test my knowledge`
- `Show my tasks`
- `Show activity log`
- `What have you done for me?`

### Activity Log

- Records significant actions with timestamps
- Logs task creation, reminders, completion, and deletion
- Logs quiz start, answers, and completion
- Shows the latest 10 actions by default
- Includes a **Show More** option for the full in-memory activity history

## Technologies Used

- C#
- .NET 8.0
- Windows Presentation Foundation (WPF)
- Newtonsoft.Json 13.0.3
- Git and GitHub
- GitHub Actions

## Requirements

- Windows 10 or Windows 11
- .NET 8 SDK
- Visual Studio Code with **C# Dev Kit**, or Visual Studio 2022
- Internet access once during restore so NuGet can download Newtonsoft.Json

Check the SDK with:

```powershell
dotnet --version
```

## Newtonsoft.Json Setup

The project file already contains this NuGet package reference:

```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

Restore it with:

```powershell
dotnet restore
```

Alternatively, the package can be added manually with:

```powershell
dotnet add package Newtonsoft.Json --version 13.0.3
```

## How to Run in VS Code

1. Clone the repository:

```powershell
git clone https://github.com/Zipho-R/CybersecurityChatbot.git
cd CybersecurityChatbot
git switch master
```

2. Open the folder:

```powershell
code .
```

3. Restore, build, and run:

```powershell
dotnet restore
dotnet build
dotnet run
```

The `greeting.wav` file is copied automatically to the output directory.

## JSON Task Storage

`TaskStorageHelper.cs` creates `tasks.json` automatically the first time a task is added. The runtime file is stored beside the compiled application, normally under a folder similar to:

```text
bin\Debug\net8.0-windows\tasks.json
```

The four storage operations are:

- **Create:** add a task and immediately rewrite `tasks.json`
- **Read:** load tasks when the application starts
- **Update:** mark a task complete or update its reminder
- **Delete:** remove the task and rewrite the JSON file

File operations are wrapped in error handling so an unavailable or invalid file does not crash the application.

## Suggested End-to-End Test

1. Launch the application and confirm the voice greeting and ASCII art.
2. Enter your name.
3. Type `I am worried about phishing`.
4. Type `tell me more`.
5. Type `Add a task to enable two-factor authentication`.
6. Reply `Yes, remind me in 5 days`.
7. Open the Task Assistant and confirm the task appears.
8. Close and reopen the application and confirm the task is still present.
9. Mark the task complete and inspect `tasks.json`.
10. Type `start quiz`, then complete all 12 questions.
11. Type `show activity log` and confirm the latest 10 timestamped actions appear.
12. Use **Show More** after generating more than 10 actions.
13. Delete a task and confirm it is removed from `tasks.json`.

## Project Structure

```text
CybersecurityChatbot
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── ChatBot.cs
├── KeywordResponder.cs
├── SentimentDetector.cs
├── MemoryStore.cs
├── AudioPlayer.cs
├── CyberTask.cs
├── TaskStorageHelper.cs
├── TaskManager.cs
├── QuizQuestion.cs
├── QuizManager.cs
├── ActivityLogger.cs
├── greeting.wav
├── CybersecurityChatbot.csproj
├── CybersecurityChatbot.sln
├── README.md
└── .github/workflows/dotnet-desktop.yml
```

## GitHub Commits and Releases

The Part 3 development history should contain at least six meaningful commits. The required release milestones are:

- `v3.0` — Task assistant and JSON storage
- `v3.1` — Quiz and activity log
- `v3.2` — NLP and final integrated POE submission

## Video Presentations

**Part 2 unlisted video:** https://youtu.be/dV-LBfXXMMo?si=JIAXsiHnxPsrl8N  
**Part 3 unlisted video:** Add final Part 3 YouTube link here before submission.

The Part 3 recording must use the student's own voice and demonstrate the running application, important code, commit history, three releases, and a successful GitHub Actions run.

## GitHub Repository

https://github.com/Zipho-R/CybersecurityChatbot

## Declaration

This project was developed for the PROG6221 Programming 2A Portfolio of Evidence. The student is responsible for reviewing, understanding, testing, and explaining all submitted code.
