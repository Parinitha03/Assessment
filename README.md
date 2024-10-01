TRAINING REPORT GENERATOR

OVERVIEW
This project is a Training Report Generator that processes training completion data and generates various reports. The application reads JSON data, analyzes training completions, and output the results into seperate files.

ENVIRONMENT SETUP
To run this project, ensure you have .NET SDK version 7 or higher installed on your machine.

RUNNING THE PROJECT
Open your command prompt or terminal
Set the directory to the folder where you have cloned the repository. 
Run the application using the command: "dotnet run"

OUTPUT
Generated output files will be located in the "Output" folder created in the project directory. You can find three reports there:
outpur_1.txt : Training completion counts
output_2.txt : Training completions by fiscal year
output_3.txt: List of prople with expired or soon-to-expire trainings by fiscal year
