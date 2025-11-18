# GetItDone-server
## Topics
- [What Is GetItDone?](#what-is-getitdone)
- [ERD](#erd)
- [Quick Start](#quick-start)
___

## What is GetItDone?
GetItDone is a lightweight task management backend built with ASP.NET Core, EF Core, PostgreSQL, and ASP.NET Identity. The goal is to demonstrate clean separation of concerns, authentication, and containerized deployment in a focused, production-style project.

## ERD
<img width="777" height="551" alt="GetItDoneERD" src="https://github.com/user-attachments/assets/afa3e072-7f10-4786-9715-0f9f624c4c71" />

## Quick Start
<p>This project runs entirely in Docker. Follow the steps below to get started:</p>
<ol>
  <li>Clone the repository:</li>
</ol>

<pre><code>git clone https://github.com/elibradford227/GetItDone-server.git
cd GetItDone-server/GetItDone
</code></pre>

<ol start="2">
  <li>Copy the example environment file:</li>
</ol>

<pre><code>cp .env.example .env
</code></pre>

<ol start="3">
  <li>Build and start the containers:</li>
</ol>

<pre><code>docker compose up --build
</code></pre>

<ol start="4">
  <li>Access the API (Swagger UI):</li>
</ol>

<pre><code>http://localhost:5000/swagger
</code></pre>



