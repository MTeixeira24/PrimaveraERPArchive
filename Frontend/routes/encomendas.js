var express = require('express');
var router = express.Router();
const request = require('request');

/* GET lista todos as encomendas. */
router.get('/', function (req, res, next) {
	if (req.session.login) {
		request.post({url: 'http://' + req.session.address + '/api/Encomendas/', form: req.session.login}, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(JSON.parse(body));
            console.log(err);
			var encomendas = JSON.parse(body);
			var encomendasPendentes = encomendas.filter(function(encomenda){
				return !(encomenda.Entregue);
			});
			res.render('pages/encomendas/index', { encomendas: encomendas, encomendasPendentes: encomendasPendentes });
		});
		} else {
        res.redirect('/login');
	}
});

/* GET informacao sobre a encomenda. */
router.get('/:encomendaID', function (req, res, next) {
    if (req.session.login) {
		request.post({ url: 'http://' + req.session.address + '/api/Encomendas/' + req.params.encomendaID, form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(body);
            console.log(err);
			res.render('pages/encomendas/show',{ encomenda: JSON.parse(body) });
		});
		} else {
        res.redirect('/login');
	}
});

module.exports = router;