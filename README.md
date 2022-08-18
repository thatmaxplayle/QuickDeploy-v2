# QuickDeploy
QuickDeploy is an easy-to-use file deployment tool written by myself for use when deploying files/folders from an IDE output folder to an installation path. It is meant as an alternative for using build events, and offers a little more customisation compared to using build events alone.

Given the fact this project started out as an internal tool, please give me some time to make the code production-application ready. The code will prboably be messy, because by no means am I dedicating this amount of time to a small project. I am simply rewriting the project for my own good, because the existing version is lacking some important fixes and features, but the code is __way__ too messy to consider just updating it. A rewrite is necessary for my own sanity. I just thought it would be nice to share a utility I wrote, but find personally useful, with other developers in the world. 

### QuickDeploy (v2) 
Version 2 the QuickDeploy system to an entirely independent application, whereas previous versions of QuickDeploy where integrated into my internal Developer Tools application (which is not publicly available)

### How it works
Set up a new deployment by specifying: 
- Directory Structure
  - Source Folder (where files are copied from)
  - Destination Folder (where files are copied to)
- Ensure Files
  > Ensure Files are files that QuickDeploy checks to ensure they exist before copying over. This functionality is ideal for game modding environments, for example, where you may have a modified and an unmodified copy of a game installed. Using another mod as an ensure file, or creating a dummy file simply for the purpose of checking you have the right installation active, prevents copying mods into an unmodified game folder!

You will then see this deployment under "My Deployments" on the main window, allowing you to quickly run the deployment.

### Roadmap
- [ ] Port the application over to WPF
  - [X] Add the ability to view existing deployments.
  - [ ] Add the ability to manage (create & remove) deployments.
 
- [ ] Application Improvements
  - [ ] Add the ability to modify deployment, adding/removing files for example or renaming, without removing and re-creating the deployment.
  - [ ] Add a seperate build counter, allowing users to track how many times a deployment has been run, allowing for accurate build numbers when publishing updates.
