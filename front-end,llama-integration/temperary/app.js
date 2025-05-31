const express = require('express');
const app = express();
const port = 3000;

app.use(express.json()); // Middleware to parse JSON

app.post("/ai-endpoint/nabonda", (req, res) => {
    const message = req.body; // Extract JSON content from body
    console.log("Received:", message);
    res.status(200).json({ result: `Processed message: ${message.input}` });
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});
