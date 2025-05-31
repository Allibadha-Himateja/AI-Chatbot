import express from 'express';
import Groq from 'groq-sdk';
import 'dotenv/config';

const app = express();
const port = 3000;

const groq = new Groq({ apiKey: process.env.GROQ_API_KEY });

app.use(express.json());

app.post("/ai/chat", async (req, res) => {
    const { input } = req.body; 

    if (!input || typeof input !== 'string') {
        return res.status(400).json({ error: "Invalid input: 'input' is required and should be a string." });
    }

    console.log("Received input:", input);

    try {
        // Call getGroqChatCompletion with the user input
        const chatResponse = await getGroqChatCompletion(input);
        const responseText = chatResponse.choices[0]?.message?.content || "No response from AI.";

        // Send response back to the client
        res.status(200).json({responseText});
    } catch (error) {
        console.error("Error processing AI chat:", error);
        res.status(500).json({ error: "Failed to process the message." });
    }
});

// Function to get chat completion from GROQ API
async function getGroqChatCompletion(userMessage) {
    return groq.chat.completions.create({
        messages: [
            {
                role: "user",
                content: userMessage,
            },
        ],
        model: "mixtral-8x7b-32768",
    });
}

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});
