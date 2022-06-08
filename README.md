# About the application
This application was developed as part of a master's dissertation on the introduction of modern technological solutions in the gamification of HR processes. The main goal of this application is to increase the interest of employees in work and their productivity. To do this, the application converts their daily performance into abstract points and employees can see both their progress and the progress of others.

Employees are rewarded for productivity with in-game achievements, as well as various rewards in real life. This idea is based on the [skinner's box](https://en.wikipedia.org/wiki/Operant_conditioning_chamber) concept.

After the successful application and implementation of gamification elements in onboarding, consent was obtained from management to develop and test the application for the company. Since the turnover rate is low, the focus of attention redirected to the main staff - active managers, in the 4 people amount.

The employees are fulfilling their plans at this moment. Their main motivation is financial. Primary signs of burnout are observed if the indicators are overfulfilled, which negatively affects the physical and emotional state.

As a result, managers open sick leave, take time off due to poor health with symptoms of acute respiratory infections. Or they work with reduced productivity and low degree of involvement first week of the month, because they worked with maximum efficiency in the previous month.

To maintain stable labor efficiency, employee incentives must be diverse. Elements of intangible motivation should be used in addition to material.

As part of the undergraduate practice, a solution was proposed in the form of software developing for a stable increase of employees working capacity, encouragement to show a personal interest and increase the degree of involvement in the process of completing tasks.

The data is entered manually by the administrator. The application was planned for Windows operating system. The software application test period is 1 month. Weekly monitoring and receiving feedback from users is planned during this time. By the end of the period, the implementation results are summed up, and a made decision on the further use of the application.

The application is developed in order to prevent burnout and maintain the pace of work of employees. The functionality is limited deliberately in order to avoid misuse of working time. At the same time, it is planned to use the program daily, involve in the idea and actively participate.

## Idea explanation
The first version of game is being developed for Metalloobrabotka company and implemented directly in the sales department. 

The main game development goal is increasing the productivity and efficiency of employees in the state and increasing the degree of loyalty to the company. The game participants are administrator, players (managers). 

The software application is a functionally-related windows with their own tasks and functions.

The employee valuation criteria entered by the administrator are broken down by relevance, totaling the unit. The administrator's task is to track the dynamics of changes in employee performance indicators, their entry into the application, content maintenance.

The application is used in passive mode, so sales managers have few active buttons, for employees. Only the administrator is actively working with the application, so employees should not be distracted from the main work.

![AdminApp](https://user-images.githubusercontent.com/57837079/172584565-4816a32d-fe4d-491a-8173-2490d42ad8c8.png)

At the beginning of working with the application, players can choose an avatar icon. In the process of application using, users can track their results and use the points earned.

Points are earned as the work plan progresses. Starting with a certain points amount, the player has the opportunity to press a special button and receive a Skinner box with game currency. It can be material values (cash premium, free coffee, company merch, etc.), as well as virtual (expansion of game functionality).

The ranking window of the standings will not contain exact amounts. Instead, the software application will use a rating for the gold silver bronze system with a corresponding increase in the number of employees. The best manager's nickname will be highlighted in a yellow box. In addition, there will be a tab of employee results for various periods of work (a week, a month, a quarter).

The player has the opportunity to see his/her page with all the data on the implementation of the work plan, his/her best indicator for the period and his/her own shelf of awards.

It should be recalled that the software application is stimulating. This functionality is designed in this way because it is important to involve personnel to work, but not to distract them from it.

Skinner's box generates its contents randomly for the employer, but at program code developing the programmer must consider the participant points amount for which the box content is generated.

## Algorithm for calculating points in the application
Admin divide the existing monthly indicators by the number of working days per month (20) and get the daily rate. Each metric is multiplied by its coefficient. Similar calculations are carried out for all indicators. The resulting values are added and multiplied by 100. Thanks to this operation, the confidential data of the company remains safe.

## Client application 
User registration is performed by the administrator. Therefore, the user enters an existing profile and selects an avatar from the proposed options.

In the application the user sees: an avatar, a place for achievements and titles, a graph of personal productivity with a change in the time range – a week, month, a quarter. The user sees the rating of employees and their distribution by prizes in the public domain. The user's nickname at the first place is highlighted in a yellow frame.

![Scoreboard](https://user-images.githubusercontent.com/57837079/172584074-daf33d80-0c49-47b1-912d-1b5f695b4dfa.png)

![Profile](https://user-images.githubusercontent.com/57837079/172584121-cf73b654-2011-4034-9f1c-0b1ad34cd528.png)

![Achievements](https://user-images.githubusercontent.com/57837079/172584150-7206976b-7966-4ca3-9d8a-cce46899f2fb.png)

## Network connection
Admin application, server script and clients application must be runned on the different machines. All interaction occurs through the server. The administrator enters private information into his application, his program calculates public points and sends them to the server. Identification of the administrator by the server is carried out by his ip.

When starting the client app, an empty GET request is sent to the server, accepting it, the server checks if the given ip is in its database. If that ip doesn't exist in database, client app will display authorization page to the user. 

If there is an ip in the database or if authorization is successful, the server response will contain an access token that is generated by the server and written to memory. This token will be verificated by the server when requesting to upload data.

When the client application's close button is clicked, a request is sent to the server to delete the token. This is how the session ends.

By the way, to start the server, you need to install python 3 and the flask library on your computer, and then, being in the folder where the script is located, write the following commands in the console

For windows:
```Bash
python main.py
```

For linux:
```Bash
python3 main.py
```

# Conclusion
This application was developed for a specific department of a specific company and the corresponding work tasks, but this concept can be applied in any industry and for any department, even based on this implementation.

The goals that this application implements are extremely important and useful in our time. According to the results of testing the application in the company during the month, the following results were obtained:

+ All employees who played exceeded their work plans by 5-15%
+ These employees noted an increased interest in the job
+ The same employees noted that the work has become much more exciting and interesting for them.
