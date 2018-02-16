var express = require('express');
var router = express.Router();
const request = require('request');

/* GET lista todos as vendas. */
router.get('/', function (req, res, next) {
    if (req.session.login) {
		request.post({url: 'http://' + req.session.address + '/api/DocVenda/', form: req.session.login}, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(JSON.parse(body));
            console.log(err);
			res.render('pages/vendas/index', { vendas: JSON.parse(body) });
		});
    } else {
        res.redirect('/login');
    }
});

/* GET informacao sobre a venda. */
router.get('/:vendaID', function (req, res, next) {
    if (req.session.login) {
		request.post({ url: 'http://' + req.session.address + '/api/DocVenda/' + req.params.vendaID, form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(body);
            console.log(err);
			res.render('pages/vendas/show',{ venda: JSON.parse(body) });
		});
    } else {
        res.redirect('/login');
    }
});

module.exports = router;
