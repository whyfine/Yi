function Monitor(interval, sleep) {
    /// <signature>
    /// <param name='interval' type='Number' optional='false' />
    /// <param name='sleep' type='Number' optional='false' />
    /// </signature>
    var sysCon = {
        //0 h5, 1 h4, 2 wxa
        sysType: window ? window.localStorage ? 0 : 1 : wx ? 2 : 3,
        interval: interval || 500,
        sleep: sleep || 5000,
        setValue: function (key, val) {
            /// <signature>
            /// <param name='key' type='String' optional='true' />
            /// <param name='val' type='Object' optional='true' />
            /// </signature>
            var allkeys = this.getAllKeys('monitorKeysStr') || [];
            if (key.indexOf('monitor_key') != -1 && allkeys.indexOf(key) == -1)
                allkeys.push(key);
            switch (this.sysType) {
                case 0:
                    localStorage.setItem('monitorKeysStr', JSON.stringify(allkeys));
                    if (typeof val == 'string')
                        localStorage.setItem(key, val);
                    else if (typeof val == 'object')
                        localStorage.setItem(key, JSON.stringify(val));
                    else
                        localStorage.setItem(key, JSON.stringify(val));
                    break;
                case 1:
                    var exdate = new Date();
                    exdate.setDate(exdate.getDate() + 100);
                    document.cookie = 'monitorKeysStr' + "=" + escape(JSON.stringify(allkeys)) +
                      ";expires=" + exdate.toGMTString()
                    if (typeof val == 'string')
                        document.cookie = key + "=" + escape(val) +
                          ";expires=" + exdate.toGMTString()
                    else if (typeof val == 'object')
                        document.cookie = key + "=" + escape(JSON.stringify(val)) +
                          ";expires=" + exdate.toGMTString()
                    else
                        document.cookie = key + "=" + escape(JSON.stringify(val)) +
                          ";expires=" + exdate.toGMTString()
                    break;
                case 2:
                    wx.setStorage({
                        key: key,
                        data: val
                    })
                    try {
                        wx.setStorageSync('monitorKeysStr', allkeys);
                    } catch (e) {
                    }

                    break;
            }
        },
        getValue: function (key) {
            /// <signature>
            /// <param name='key' type='String' optional='true' />
            /// </signature>
            var val;
            switch (this.sysType) {
                case 0:
                    var t = localStorage.getItem(key);
                    if (t && t != null)
                        try {
                            val = JSON.parse(t);
                        } catch (e) {
                            val = t;
                        }
                    else
                        val = t;
                    break;
                case 1:
                    if (document.cookie.length > 0) {
                        var start = document.cookie.indexOf(key + "=")
                        if (start != -1) {
                            start = start + key.length + 1
                            end = document.cookie.indexOf(";", start)
                            if (end == -1) end = document.cookie.length
                            var t = unescape(document.cookie.substring(start, end))
                            if (t && t != null)
                                try {
                                    val = JSON.parse(t);
                                } catch (e) {
                                    val = t;
                                }
                            else
                                val = t;
                        }
                    }
                    break;
                case 2:
                    try {
                        val = wx.getStorageSync(key)
                    } catch (e) {
                    }
                    break;
            }
            return val;
        },
        removeValue: function (key) {
            /// <signature>
            /// <param name='key' type='String' optional='true' />
            /// </signature>
            var allkeys = this.getAllKeys();
            var index = allkeys.indexOf(key);
            if (index != -1) {
                allkeys.splice(index, 1);
            }
            switch (this.sysType) {
                case 0:
                    localStorage.removeItem(key);
                    localStorage.setItem('monitorKeysStr', JSON.stringify(allkeys));
                    break;
                case 1:
                    var exdate = new Date();
                    exdate.setDate(exdate.getDate() + -1);
                    var noexdate = new Date();
                    noexdate.setDate(noexdate.getDate() + 100);
                    document.cookie = 'monitorKeysStr' + "=" + escape(JSON.stringify(allkeys)) +
                      ";expires=" + noexdate.toGMTString();
                    document.cookie = key + "=" + escape('') +
                      ";expires=" + exdate.toGMTString();
                    break;
                case 2:
                    try {
                        wx.removeStorageSync(key);
                    } catch (e) {
                    }
                    try {
                        wx.setStorageSync('monitorKeysStr', allkeys);
                    } catch (e) {
                    }
                    break;
            }
        },
        getAllKeys: function () {
            return this.getValue('monitorKeysStr') || new Array();
        },
        http: function (url, method, data, success, error) {
            /// <signature>
            /// <param name='url' type='String' optional='true' />
            /// <param name='type' type='String' optional='true' />
            /// <param name='data' type='Object' optional='true' />
            /// <param name='success' type='Function' optional='false' />
            /// <param name='error' type='Function' optional='false' />
            /// </signature>
            switch (this.sysType) {
                case 0:
                case 1:
                    var xmlHttp = null;
                    if (window.XMLHttpRequest) {// code for IE7, Firefox, Opera, etc.
                        xmlHttp = new XMLHttpRequest();
                    }
                    else if (window.ActiveXObject) {// code for IE6, IE5
                        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                    }
                    if (xmlHttp != null) {
                        xmlHttp.onreadystatechange = function () {
                            if (xmlHttp.readyState == 4) {// 4 = "loaded"
                                if (xmlHttp.status == 200) {// 200 = OK
                                    if (success) success();
                                }
                                else {
                                    if (error) error();
                                }
                            }
                        };
                        xmlHttp.open(method, url, true);
                        xmlHttp.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                        xmlHttp.send(JSON.stringify(data));
                    }
                    else {
                        throw 'Your browser does not support XMLHTTP.';
                    }
                    break;
                case 2:
                    wx.request({
                        url: url, //仅为示例，并非真实的接口地址
                        method: method,
                        data: data,
                        header: {
                            'content-type': 'application/json' // 默认值
                        },
                        success: function (d) {
                            if (success) success(d);
                        },
                        fail: function (d) {
                            if (error) error(d);
                        }
                    })
                    break;
            }

        },
        guid: function () {
            function S4() {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            }
            return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
        },
        queryByAttribute: function (attr, root, arr) {
            root = root || document.body;
            arr = arr || new Array();
            if (root.hasAttribute(attr)) {
                arr.push(root);
            }
            var children = root.children,
              element;
            for (var i = children.length; i--;) {
                this.queryByAttribute(attr, children[i], arr);
            }
            return arr;
        }
    };

    var monitor_obj = {
        eventType: { onLoad: 0, onShow: 1, onUnLoad: 2, onPageScroll: 3, onRegister: 101, onLogin: 102, onAddColl: 103, onAddOrder: 104, onPayOrder: 105, onAddComment: 106 },
        page: { pageId: '', pageData: { ChannelId: 0, ChannelTitle: '', ContentId: 0, ContentTitle: '' }, requestDatas: [] },
        getConfigs: function () {
            var configs = sysCon.getValue('monitor_configs');
            if (!configs) {
                configs = { CookieId: sysCon.guid(), UserId: '' };
                sysCon.setValue('monitor_configs', configs);
            }
            return configs;
        },
        setConfigs: function (configs) {
            sysCon.setValue('monitor_configs', configs);
        },
        addMonitor: function (data, eventType) {
            /// <signature>
            /// <param name='data' type='Object' optional='true' />
            /// <param name='eventType' type='String' optional='true' />
            /// </signature>
            if (data) {
                data.PageId = this.page.pageId;
                data.MonitorTimestamp = Math.round(new Date().getTime());
                data.EventType = eventType;
                sysCon.setValue('monitor_key' + sysCon.guid(), data);
            }
        },
        initPage: function (channelId, channelTitle, contentId, contentTitle, data) {
            /// <signature>
            /// <param name='channelId' type='Number' optional='false' />
            /// <param name='channelTitle' type='String' optional='false' />
            /// <param name='contentId' type='Number' optional='false' />
            /// <param name='contentTitle' type='String' optional='false' />
            /// <param name='data' type='Object' optional='false' />
            /// </signature>
            if (sysCon.sysType == 0 || sysCon.sysType == 1) {
                //onload
                var onload = window.onload;
                window.onload = function () {
                    monitor_obj.onLoad();
                    if (onload) onload();
                    initOnClick();
                    intiPageShow();
                }
                //onunload
                var onunload = window.onunload;
                window.onunload = function () {
                    monitor_obj.onUnLoad(channelId, channelTitle, contentId, contentTitle);
                    if (onunload) onunload();
                }
                //onscroll
                var onscroll = window.onscroll;
                window.onscroll = function () {
                    var scrollTop = 0;
                    if (document.documentElement && document.documentElement.scrollTop) {
                        scrollTop = document.documentElement.scrollTop;
                    }
                    else if (document.body) {
                        scrollTop = document.body.scrollTop;
                    }
                    monitor_obj.onPageScroll(scrollTop);
                    if (onscroll) onscroll();
                }
                //onlick
                function initOnClick() {
                    var clickArr = sysCon.sysType == 0 ? document.querySelectorAll('[monitor-click]') : sysCon.queryByAttribute('monitor-click');
                    if (clickArr)
                        clickArr.forEach(function (item) {
                            var onclick = item.onclick;
                            item.onclick = function () {
                                var str = item.getAttribute("monitor-click");
                                var d;
                                if (str) {
                                    try {
                                        d = JSON.parse(str);
                                    }
                                    catch (e) {
                                    }
                                    if (d && d.eventName && d.eventData)
                                        monitor_obj.onClick(d.eventData, d.eventName);
                                }
                                if (onclick) onclick(d);
                            };
                        });
                }
                //onshow
                function intiPageShow() {
                    var onshow = document.body.onpageshow;
                    document.body.onpageshow = function () {
                        monitor_obj.onShow(channelId, channelTitle, contentId, contentTitle);
                        if (onshow) onshow();
                    }
                }
            }
            else if (sysCon.sysType == 2 && data) {
                monitor_obj.trigger = function (eventType, data) {
                    var pages = getCurrentPages(),
                      curPage = pages[pages.length - 1],
                      callback = curPage[eventType];
                    callback && callback.call(curPage, data);
                }
                //onload
                var onload = data.onload;
                data.onload = function (e) {
                    monitor_obj.onLoad();
                    if (onload) onload(e);
                }
                //onshow
                var onshow = data.onShow;
                data.onShow = function (e) {
                    monitor_obj.onShow(channelId, channelTitle, contentId, contentTitle);
                    if (onshow) onshow(e);
                }
                //onunload
                var onunload = data.onUnload;
                data.onUnload = function (e) {
                    monitor_obj.onUnLoad();
                    if (onunload) onunload(e);
                }
                //onscroll
                var onscroll = data.onPageScroll;
                data.onPageScroll = function (e) {
                    monitor_obj.onPageScroll(e.scrollTop);
                    if (onscroll) onscroll(e);
                }
                //onlick
                data.monitortap = function (e) {
                    if (e.currentTarget.dataset.monitor) {
                        var d;
                        try {
                            d = JSON.parse(e.currentTarget.dataset.monitor);
                        } catch (e) {

                        }
                        if (d) {
                            if (d.tap)
                                monitor_obj.trigger(d.tap, d);
                            if (d.eventName && d.eventData)
                                monitor_obj.onClick(d.eventData, d.eventName);
                        }
                    }
                }
                return data;
            }
        },
        onLoad: function () {
            /// <signature>
            /// <param name='channelId' type='Number' optional='false' />
            /// <param name='channelTitle' type='String' optional='false' />
            /// <param name='contentId' type='Number' optional='false' />
            /// <param name='contentTitle' type='String' optional='false' />
            /// </signature>
            this.page.pageId = sysCon.guid();
            this.addMonitor(this.page.pageData, this.eventType.onLoad);
        },
        onShow: function (channelId, channelTitle, contentId, contentTitle) {
            /// <signature>
            /// <param name='channelId' type='Number' optional='false' />
            /// <param name='channelTitle' type='String' optional='false' />
            /// <param name='contentId' type='Number' optional='false' />
            /// <param name='contentTitle' type='String' optional='false' />
            /// </signature>
            this.page.pageData = { ChannelId: channelId || 0, ChannelTitle: channelTitle || '', ContentId: contentId || 0, ContentTitle: contentTitle || '' };
            this.addMonitor(this.page.pageData, this.eventType.onShow);
        },
        onUnLoad: function () {
            this.addMonitor(this.page.pageData, this.eventType.onUnLoad)
        },
        onPageScroll: function (scrollHeight) {
            this.page.pageData.ScrollHeight = scrollHeight;
            this.addMonitor(this.page.pageData, 'onPageScroll')
        },
        onClick: function (eventData, eventName) {
            var et = this.eventType[eventName];
            if (et || et == 0) {
                if (!this.page.requestDatas[et]) {
                    eventData.RequestId = sysCon.guid();
                    this.page.requestDatas[et] = eventData;
                }
                this.addMonitor(eventData, et)
            }
        },
        onLogin: function (userid, result) {
            //{UName:'',Mobile:''}
            /// <signature>
            /// <param name='uid' type='String' optional='false' />
            /// <param name='result' type='Boolean' optional='false' />
            /// </signature>
            if (userid) {
                var configs = this.getConfigs();
                configs.UserId = userid;
                this.setConfigs(configs);
            }
            var eventData = this.page.requestDatas[this.eventType.onLogin];
            if (eventData) {
                eventData.Result = result || true;
                this.page.requestDatas.splice(this.eventType.onLogin, 1);
                this.addMonitor(eventData, this.eventType.onLogin);
            }
        },
        onRegister: function (userid, result) {
            //{UName:'',Mobile:''}
            /// <signature>
            /// <param name='uid' type='String' optional='false' />
            /// <param name='result' type='Boolean' optional='false' />
            /// </signature>
            if (userid) {
                var configs = this.getConfigs();
                configs.UserId = userid;
                this.setConfigs(configs);
            }
            var eventData = this.page.requestDatas[this.eventType.onRegister];
            if (eventData) {
                eventData.Result = result || true;
                this.page.requestDatas.splice(this.eventType.onRegister, 1);
                this.addMonitor(eventData, this.eventType.onRegister);
            }
        },
        onAddColl: function (result) {
            //{ContentId:'',ContentTitle:''}
            /// <signature>
            /// <param name='result' type='Boolean' optional='false' />
            /// </signature>
            var eventData = this.page.requestDatas[this.eventType.onAddColl];
            if (eventData) {
                eventData.Result = result || true;
                this.page.requestDatas.splice(this.eventType.onAddColl, 1);
                this.addMonitor(eventData, this.eventType.onAddColl);
            }
        },
        onAddOrder: function (orderiId, result) {
            //{ OrderValue:  0, Province:  '', City: '', Area:  '', Details: [{contentId:1,Num:1}]}
            /// <signature>
            /// <param name='orderiId' type='String' optional='false' />
            /// <param name='result' type='Boolean' optional='false' />
            /// </signature>
            var eventData = this.page.requestDatas[this.eventType.onAddOrder];
            if (eventData) {
                eventData.OrderId = orderiId;
                eventData.Result = result || true;
                this.page.requestDatas.splice(this.eventType.onAddOrder, 1);
                this.addMonitor(eventData, this.eventType.onAddOrder);
            }
        },
        onPayOrder: function (orderId, result) {
            //{ PayValue:  0, PayWay:0}
            /// <signature>
            /// <param name='orderiId' type='String' optional='false' />
            /// <param name='result' type='Boolean' optional='false' />
            /// </signature>
            var eventData = this.page.requestDatas[this.eventType.onPayOrder];
            if (eventData) {
                eventData.OrderId = orderId;
                eventData.Result = result || true;
                this.page.requestDatas.splice(this.eventType.onPayOrder, 1);
                this.addMonitor(eventData, this.eventType.onPayOrder);
            }
        },
        onAddComment: function (result) {
            //{ ContentId:  0, Level:0,Content:'',HasImg:true}
            /// <signature>
            /// <param name='orderiId' type='String' optional='false' />
            /// <param name='result' type='Boolean' optional='false' />
            /// </signature>
            var eventData = this.page.requestDatas[this.eventType.onAddComment];
            if (eventData) {
                eventData.Result = result || true;
                this.page.requestDatas.splice(this.eventType.onAddComment, 1);
                this.addMonitor(eventData, this.eventType.onAddComment);
            }
        }
    };

    function start(timeout) {
        setTimeout(function () {
            var arr = sysCon.getAllKeys();
            var key = arr.length > 0 ? sysCon.getAllKeys()[0] : '';
            if (key) {
                var configs = monitor_obj.getConfigs();
                var data = sysCon.getValue(key);
                data.CookieId = configs.CookieId;
                data.UserId = configs.UserId;
                sysCon.http('http://localhost:16694/api/monitor', 'POST', data, function (d) {
                    sysCon.removeValue(key);
                    start(sysCon.interval);
                }, function (d) { start(sysCon.interval); });
            }
            else
                start(sysCon.sleep)
        }, timeout);
    }

    start(500);

    return monitor_obj;
}
//module.exports = new Monitor();



////html
////js
// var jc = new Monitor();
// jc.initEvent();

//html
//<button type="button" monitor-click='{"ClickTxt":"click me","Target":"baidu.com"}'>click me</button>


////vxa
////js
//var jc = require('../../utils/monitor.js')
//Page(jc.initEvent('pages/index/index','登陆页','{channel:10,skuid:10}',{
//    data: {
//        motto: 'Hello World',
//        userInfo: {},
//        hasUserInfo: false,
//        canIUse: wx.canIUse('button.open-type.getUserInfo')
//    },
//    clickMe: function (e) {
//        console.log('hello');
//    },}))

////view
//<button bindtap="monitortap" data-monitor='{"tap":"clickMe","ClickTxt":"click me","Target":"baidu.com"}'>登陆</button>