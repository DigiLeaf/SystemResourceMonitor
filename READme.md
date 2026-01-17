# System Resource Monitor
System Resource Monitor is a lightweight C#/.NET console application that periodically monitors local system resources, including CPU, memory, and disk usage.
Collected metrics are stored in an Azure SQL Database to enable historical analysis, reporting, and alerting.

## Features (Planned)
- Monitors local system resources:
	- CPU usage
	- Memory (RAM) usage
	- Disk Usage
- Periodic data collection at configurable intervals
- Persistent storage using Azure SQL Database
- Historical data querying and basic reporting
- Threshold-based alerts for abnormal resource usage
- Export monitoring data to Excel files

## Tech Stack
- **Lanuage**
	- C#
- **Runtime**
  - .NET (Console Application)
- **Database**
  - Azure SQL Database
- **Data Access** 
  - Entity Framework Core
- **Version Control**
  - Git & Github

## Project Structure
- Alerts/: Handles threshold detection and alerts
- Config/: Stores application configuration 
- Data/: Manages database access and persistence
- Logs/: Records operational history
- Monitoring/: Collects raw system metrics
- Reporting/: Generates summaries from collected metrics