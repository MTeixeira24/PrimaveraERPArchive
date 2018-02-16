var express = require('express');
var router = express.Router();

/* GET pagina ganhos. */
router.get('/', function (req, res, next) {
    if (req.session.login) {
        res.render('pages/ganhos/index');
    } else {
        res.redirect('/login');
    }
});

module.exports = router;
