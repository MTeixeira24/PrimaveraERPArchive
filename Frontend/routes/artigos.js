var express = require('express');
var router = express.Router();
const request = require('request');

/* GET lista todos artigos. */
router.get('/', function (req, res, next) {
    if (req.session.login) {
        console.log(req.session.login);
        request.post({url: 'http://' + req.session.address + '/api/artigos/', form: req.session.login}, function (err, response, body) {
            if (err) { res.status(400).send() }
            //console.log(JSON.parse(body));
            //console.log(err);
            maxStock = 10;
            console.log(req.query.maxStock);
            if (req.query.maxStock != null) {
                maxStock = req.query.maxStock;
            }
            console.log(maxStock);
            var artigos = JSON.parse(body);
            var artigosSemStock = artigos.filter(function (obj) {
                return obj.STKAtual == 0;
            });
            var artigosMaxStock = artigos.filter(function (obj) {
                return obj.STKAtual <= maxStock;
            });


            res.render('pages/artigos/index', { artigos: artigos, artigosSemStock: artigosSemStock, artigosMaxStock : artigosMaxStock });
        });
    } else {
        res.redirect('/login');
    }
});


/* GET informacao sobre artigo. */
router.get('/:codArtigo', function (req, res, next) {
    if (req.session.login) {
        request.post({ url: 'http://' + req.session.address + '/api/artigos/' + req.params.codArtigo, form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            //console.log(body);
            //console.log(err);
            var artigo = JSON.parse(body);
			let tran = artigo.transaccoes
            let vendas = tran.filter(function(obj){
				return obj.tipo
			});
            let compras = tran.filter(function(obj){
				return !obj.tipo
			});
            res.render('pages/artigos/show', { artigo: artigo, vendas: vendas, compras: compras });
        })
    } else {
        res.redirect('/login');
    }
});

module.exports = router;
