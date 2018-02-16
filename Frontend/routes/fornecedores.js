var express = require('express');
var router = express.Router();
const request = require('request');

/* GET lista todos os fornecedores. */
router.get('/', function (req, res, next) {
    if (req.session.login) {
		request.post({url: 'http://' + req.session.address + '/api/Fornecedor/', form: req.session.login}, function (err, response, body) {
            if (err) { res.status(400).send() }
			console.log(JSON.parse(body));
			console.log(err);
			res.render('pages/fornecedores/index', { fornecedores: JSON.parse(body) });
		});
    } else {
        res.redirect('/login');
    }
});

/* GET informacao sobre o fornecedor. */
router.get('/:fornecedorID', function (req, res, next) {
    if (req.session.login) {
		request.post({ url: 'http://' + req.session.address + '/api/Fornecedor/' + req.params.fornecedorID, form: req.session.login }, function (err, response, body) {
            if (err) { res.status(400).send() }
            console.log(err);
            var fornecedor = JSON.parse(body);
			// get dos artigos a partir dos docCompra e Encomenda
            tmpArtigos = [];
            (fornecedor.Compras).forEach(function(compra){
                (compra.LinhasDoc).forEach(function (linha) {
                    if (typeof tmpArtigos[linha.CodArtigo] == 'undefined') {
                        tmpArtigos[linha.CodArtigo] = linha;
                        tmpArtigos[linha.CodArtigo].nArtigos = 1;
                    } else {
                        tmpArtigos[linha.CodArtigo].nArtigos++;
                    }
                });
            });
			(fornecedor.Encomendas).forEach(function(encomenda){
                (encomenda.LinhasEnc).forEach(function (linha) {
                    if (typeof tmpArtigos[linha.CodArtigo] == 'undefined') {
                        tmpArtigos[linha.CodArtigo] = linha;
                        tmpArtigos[linha.CodArtigo].nArtigos = 1;
                    } else {
                        tmpArtigos[linha.CodArtigo].nArtigos++;
                    }
                });
            });
			fornecedor.artigos = Object.keys(tmpArtigos).map(function(key){return tmpArtigos[key]});
            console.log(fornecedor);
            res.render('pages/fornecedores/show', { fornecedor: fornecedor });
		});
    } else {
        res.redirect('/login');
    }
});

module.exports = router;
