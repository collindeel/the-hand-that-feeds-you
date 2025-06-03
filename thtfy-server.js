const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const fs = require('fs');

const app = express();
app.use(cors());
app.use(bodyParser.json());

app.post('/submit-score', (req, res) => {
    const { scene, name, score } = req.body;
    console.log(`New score: ${name} - ${score}`);

    // Save to file (or database)
    fs.appendFileSync(`scores.txt`, `${name},${score}\n`);
    res.json({ status: 'success' });
});

app.get('/get-scores', (req, res) => {
    const sanitizeFilename = (name) => name.replace(/[^a-zA-Z0-9_-]/g, '');
    const filename = `scores.txt`;
    if (!fs.existsSync(filename)) {
       return res.json([]);
    }
    const data = fs.readFileSync(filename, 'utf-8');
    const lines = data.trim().split('\n');
    const scores = lines.map(line => {
        const [name, score] = line.split(',');
        return { name, score: parseInt(score) };
    });
    res.json(scores);
});


app.listen(3001, () => console.log('Score server running on port 3001'));
