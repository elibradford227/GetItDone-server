# GetItDone-server

<h2>Quick Start</h2>
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

