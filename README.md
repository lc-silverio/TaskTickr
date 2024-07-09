![TaskTickr logo](./TaskTickr/Logo.png)

## Purpose
- TaskTickr is a Jira companion app that allows users to easily track and log time spent on tasks directly from their desktop. 
- This tool streamlines the process of time management and task tracking, enhancing productivity and ensuring accurate logging of work hours.
  
## Features
- Easy time tracking: With a classic timer interface you can easily start and stop the timer as well as view your progress in a single place  
- Direct logging: Log time spent on tasks directly into Jira without leaving your desktop
- Task filtering: Filter tasks based on your criteria for easy access and management
- Customizable settings: Adjust the filter to your needs and get a tailored list of tasks that suits you needs
- Report generator: TaskTickr also generates a CSV file with all your logged task that can be imported and analised in any CSV compatible platform

## Table of Contents
- [Purpose](#purpose)
- [Features](#features)
- [Table of Contents](#table-of-contents)
- [Getting Started](#getting-started)
  - [Installation](#installation)
  - [Configuration](#configuration)
- [How to use](#how-to-use)
- [Contributing](#contributing)
- [Feedback and Support](#feedback-and-support)
- [Enjoying TaskTickr?](#enjoying-tasktickr)

## Getting Started

### Installation
1. Download the latest release from the GitHub releases page
2. Run the installer and follow the install steps
3. Configure your Jira credentials and task filters in the TaskTickrs settings file

### Configuration
1. Navigate to the TaskTickr install folder. Default location is `%LocalAppData%\TaskTickr`
2. Open the TaskTickrSettings.ini file in your text editor of choice
3. Update the following sections with your Jira credentials and desired task filters

```ini
    [JiraCredentials]
    TargetInstanceURL = https://instance.atlassian.net
    Username = user@domain.com
    APIKey = insert_personal_api_token_key_here

    [TaskFilters]
    ExcludedTaskStatus = Declined, Canceled, Ready for testing, Released to Test, Ready for Deployment, Released to Production, Done, Closed
```

## How to use
1. Launch TaskTickr from your desktop or start menu
2. Select the desired task from the drop-down menu
3. Click on the `Start` button to start the timer
4. When desired click on the `Stop` button to log you work time and reset the timer

**Notes**
- TaskTickr will not log less than 1 minute of work (Sorry Flash)
- TaskTickr will log your work time into a CSV file located in the install directory (`.\TaskTickr\logs\worklog.csv`)
- TaskTickr logs application errors into a TXT file located in the (`.\TaskTickr\logs\log.csv`)

## Contributing
- If you would like to contribute to the development of TaskTickr, first of all thank you for your interest and secondly please fork the repository and submit a pull request
- For major changes, please open an issue first to discuss what you would like to change

## Feedback and Support
- If you encounter any issues or have suggestions for improvements, please open an issue on the GitHub repository and i'll look into it as soon as possible

## Enjoying TaskTickr?
- If you find TaskTickr helpful, please consider giving it a star on GitHub. Your support is greatly appreciated!
- Thank you for using TaskTickr! I hope it enhances your productivity and makes your time tracking easier