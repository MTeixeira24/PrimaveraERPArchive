var express = require('express');
var router = express.Router();
const request = require('request');

/* GET home page. */
router.get('/', function (req, res, next) {
    if (req.session.login) {
        request.post({ url: 'http://' + req.session.address + '/api/homepage/', form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            res.render('pages/index', { homepage: JSON.parse(body) });
        });
    } else {
        res.redirect('/login');
    }
});

/* GET login page. */
router.get('/login', function (req, res, next) {
    if (req.session.login) {
        res.redirect('/');
    } else {
        res.render('pages/login');
    }
});

/* POST login. */
router.post('/login', function (req, res, next) {
    if (req.session.login) {
        res.status(400).send();
    } else {
        var form = {
            company: req.body.company,
            user: req.body.user,
            password: req.body.password
        };
        request.post({ url: 'http://' + req.body.address + '/api/login/', form: form }, function (err, response, body) {
            if (err) {
                res.status(400).redirect('/login');
            } else if (body == "false") {
                res.status(400).redirect('/login');
            } else {
                req.session.login = {};
                req.session.login.company = req.body.company;
                req.session.login.user = req.body.user;
                req.session.login.password = req.body.password;
                req.session.address = req.body.address;
                res.cookie('login', req.session.login).status(200).redirect('/');
            }
            //res.render('pages/artigos/index', { artigos: body });
        })

    }
});

/* POST refresh saft. */
router.post('/refresh', function (req, res, next) {
    if (req.session.login) {
        request.post({ url: 'http://' + req.session.address + '/api/parser/' + req.body.saftPath, form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            //console.log(JSON.parse(body));
            console.log(err);
            res.redirect('/');
        });
    } else {
        res.redirect('/login');
    }
});

/* GET logout. */
router.get('/logout', function (req, res, next) {
    req.session.destroy(function (err) {
        res.clearCookie('login').redirect('/login');
    });
});

module.exports = router;
