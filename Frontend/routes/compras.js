var express = require('express');
var router = express.Router();
const request = require('request');

/* GET lista todas as compras. */
router.get('/', function (req, res, next) {
    if (req.session.login) {
		request.post({url: 'http://' + req.session.address + '/api/DocCompra/', form: req.session.login}, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(JSON.parse(body));
            console.log(err);
			res.render('pages/compras/index', { compras: JSON.parse(body) });
		});
    } else {
        res.redirect('/login');
    }
});

/* GET informacao sobre a compra. */
router.get('/:compraID', function (req, res, next) {
    if (req.session.login) {
		request.post({ url: 'http://' + req.session.address + '/api/Compras/' + req.params.compraID, form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(body);
            console.log(err);
			res.render('pages/compras/show',{ compra: JSON.parse(body) });
		});
		} else {
        res.redirect('/login');
	}
});

module.exports = router;
