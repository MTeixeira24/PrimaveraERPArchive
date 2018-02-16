var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var session = require('express-session');

var index = require('./routes/index');
var artigos = require('./routes/artigos');
var clientes = require('./routes/clientes');
var compras = require('./routes/compras');
var encomendas = require('./routes/encomendas');
var fornecedores = require('./routes/fornecedores');
var vendas = require('./routes/vendas');
var ganhos = require('./routes/ganhos');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');

// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));
app.use(session({
    secret: 'sinf',
    saveUninitialized: false,
    resave: false,
    maxAge: 3600000
}));

app.use(function(req, res, next) {
  res.locals.login = req.session.login;
  next();
});

app.use('/', index);
app.use('/artigos', artigos);
app.use('/clientes', clientes);
app.use('/compras', compras);
app.use('/encomendas', encomendas);
app.use('/fornecedores', fornecedores);
app.use('/vendas', vendas);
app.use('/ganhos', ganhos);

// 
app.use('/css/gentelella.min.css', express.static(path.join(__dirname, '/node_modules/gentelella/build/css/custom.min.css')));
app.use('/js/gentelella.min.js', express.static(path.join(__dirname,'/node_modules/gentelella/build/js/custom.min.js')));
app.use('/vendors', express.static(path.join(__dirname, '/node_modules/gentelella/vendors')));

app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
