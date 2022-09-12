const express = require('express');
var bodyParser = require('body-parser');
const fs = require('fs');
const app = express();
const port = 3000;
var scores = null;
var scoresFile = "scores.json";
app.set("view engine", "ejs");

// configure the app to use bodyParser() for JSON stuff
app.use(bodyParser.urlencoded({
    extended: true
}));
app.use(bodyParser.json());

app.put('/submit', (req, res) => {
    if (req.is("json")) {
        var obj = req.body;
        //console.log(`name: ${obj['name']}, score: ${obj['score']}, enemy: ${obj['enemy']}`);
        if ('name' in obj && 'score' in obj && 'enemy' in obj &&
            typeof(obj['name']) === "string" && typeof(obj['score']) === "number" && typeof(obj['enemy']) === "string") {
            addScore(obj);
            res.sendStatus(200);
        }
        else {
            res.sendStatus(400);
        }
    }
    else {
        res.sendStatus(406);
    }
});

function addScore(scoreObject) {
    scores.push(scoreObject);
    scores = scores.sort((a, b) => a.score < b.score);

    if (scores.length > 5) {
        scores = scores.slice(0, 5);
    }
    fs.writeFile(scoresFile, JSON.stringify(scores), (err) => {
        if (err) { console.log(`Failed to write ${scoresFile}, error: ${err}`); }
    })
}

app.get('/scores', (req, res) => {
    if ("type" in req.query && req.query["type"] == "text") {
        var response_string = "";
        scores.forEach(score => {
            response_string += `"${score.score}","${score.enemy}","${score.name}"\n`;
        });
        res.status(200).send(response_string);
    }
    else {
        res.status(200).json(scores);
    }
});
app.get('/', (req, res) => {
    res.render('leaderboard', {scores: scores});
});

app.listen(port, () => {
    app.use(express.static("public"));
    fs.access(scoresFile, fs.constants.F_OK, (err) => {
        if (!err) {
            let rawdata = fs.readFileSync(scoresFile);
            scores = JSON.parse(rawdata);
        }
        else {
            scores = [];
        }
    });
    console.log(`Example app listening on port ${port}`)
});