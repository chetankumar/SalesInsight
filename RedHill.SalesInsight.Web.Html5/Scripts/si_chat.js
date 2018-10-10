/*global TChat */
(function () {
    //Constructor
    this.TChat = function () {
        // Create global element references
        this.closeButton = null;
        this.modal = null;
        this.overlay = null;
        this.chatMessage = null;
        this.chatConv = null;
        this.chats = [];
        this.toggleLink = null;

        // Determine proper prefix
        this.transitionEnd = transitionSelect();

        // Define option defaults
        var defaults = {
            className: 'fade-and-drop',
            closeButton: true,
            content: "",
            maxWidth: 200,
            minWidth: 280,
            sendButton: false,
            overlay: true,
            chatPlaceHolder: "Type in and hit enter to send",
            userName: null
        }

        // Create options by extending defaults with the passed in arugments
        if (arguments[0] && typeof arguments[0] === "object") { //jslint: ignore line
            this.options = extendDefaults(defaults, arguments[0]); //jslint: ignore line
        }

        this.open();
        this.close();
    }

    TChat.prototype.open = function () {
        buildOut.call(this);
        initializeEvents.call(this);
        window.getComputedStyle(this.modal).height;
        this.modal.className = this.modal.className +
        (this.modal.offsetHeight > window.innerHeight ?
        " sichat-open sichat-anchored" : " sichat-open");
        if (this.overlay) {
            this.overlay.className = this.overlay.className + " sichat-open";
        }
    }

    TChat.prototype.refresh = function () {

    }

    TChat.prototype.close = function () {
        var _ = this;
        this.modal.className = this.modal.className.replace(" sichat-open", "");
    }

    TChat.prototype.show = function () {
        var _ = this;
        if (_.modal.className.indexOf("sichat-open") < 0) {
            _.modal.className = _.modal.className + " sichat-open";
            _.chatConv.scrollTop = _.chatConv.scrollHeight;
        }
    }

    TChat.prototype.getConversationId = function () {
        return document.querySelector("#Chat_ConvId").value;
    }

    TChat.prototype.showLoading = function () {
        var loader = document.querySelector(".sichat-box .sichat-loader");
        if (loader.className.indexOf("open") < 0) {
            loader.className = loader.className + " open";
        }
    }

    TChat.prototype.hideLoading = function () {
        var loader = document.querySelector(".sichat-box .sichat-loader");
        if (loader.className.indexOf("open") >= 0) {
            loader.className = loader.className.replace("open", "").trim();
        }
    }


    TChat.prototype.sendMessage = function (e) {
        var _ = this;
        if (e.which === 13 && this.chatMessage.value.length > 0 && e.shiftKey === false) {
            var item = this;
            var msg = this.chatMessage.value;
            var data = [];
            var _ = this;
            console.log("Sending Message");
            $.ajax({
                url: "/Json/SendMessage",
                method: 'POST',
                data: { conversationId: _.getConversationId(), message: msg },
                success: function (res) {
                    if (res) {
                        var data = [];
                        data.push(res);
                        renderMessages(_, data);
                        item.chatMessage.value = "";
                        if (chatHub) {
                            console.log("Sending Message");
                            chatHub.server.send(res.UserName, res.Message);
                        }
                        _.chatConv.scrollTop = _.chatConv.scrollHeight;
                    }
                }
            });
            return true;
        }
    }

    TChat.prototype.ignoreKeys = function (e) {
        if (e.which === 13 && e.shiftKey === false) {
            e.preventDefault();
            return false;
        }
    }

    TChat.prototype.showMessages = function (data) {
        this.chatConv.innerHTML = "";
        if (!data.length) {
            renderNoMessages(this);
        } else {
            renderMessages(this, data);
        }
        this.chatConv.scrollTop = this.chatConv.scrollHeight;
    }

    TChat.prototype.showNewMessages = function (data) {
        renderMessages(this, data);
        if (data && data.length) {
            this.chatConv.scrollTop = this.chatConv.scrollHeight;
        }
    }

    TChat.prototype.getLastMessageId = function () {
        var items = document.querySelectorAll(".sichat-msg.other");
        var last = "";
        if (items && items.length) {
            last = items[items.length - 1];
        }
        if (last != "") {
            return last.getAttribute("data-id");
        }
        return null;
    }

    function renderNoMessages(t) {
        t.chatConv.innerHTML = "";
        var msg = t.options.emptyMessage;
        if (!msg) {
            msg = "No available messages";
        }
        var emptyChat = document.createElement("div");
        emptyChat.innerHTML = msg;
        emptyChat.className = "sichat-no-message";

        t.chatConv.appendChild(emptyChat);
    }

    function removeEmptyMessage() {
        var emptyMessage = document.querySelector(".sichat-no-message");
        if (emptyMessage) {
            emptyMessage.remove();
        }
    }

    function renderMessages(t, data) {
        var userName = t.options.userName;
        if (data.length) {
            removeEmptyMessage();
            var isNotif = false;
            for (var i = 0; i < data.length; i++) {
                var d = data[i];
                var exists = document.querySelectorAll(".sichat-msg[data-id='" + d.Id + "']").length;
                if (exists) {
                    continue;
                }
                var chatDiv = document.createElement("div");
                chatDiv.innerText = d.Message;
                chatDiv.className = "sichat-msg";

                isNotif = (d.UserName == null || d.UserName === undefined);

                var dateTime = moment(d.TimeStamp, "YYYYMMDDHHmm");

                if (isNotif) {
                    chatDiv.className = chatDiv.className + " notif";
                    chatDiv.setAttribute("title", dateTime.format("[on] MM/DD/YYYY [at] hh:mm a"));
                } else {
                    if (userName === d.UserName) {
                        chatDiv.className = chatDiv.className + " me";
                    } else {
                        chatDiv.className = chatDiv.className + " other";
                    }
                    var detDiv = document.createElement("div");

                    var userDiv = document.createElement("span");
                    userDiv.className = "sichat-user";
                    userDiv.innerHTML = d.UserName;
                    detDiv.appendChild(userDiv);

                    var timeDiv = document.createElement("span");
                    var time = dateTime.fromNow();
                    timeDiv.setAttribute("title", dateTime.format("MM/DD/YYYY hh:mm a"));
                    timeDiv.innerHTML = time;
                    timeDiv.className = "sichat-timestamp";
                    detDiv.appendChild(timeDiv);
                    chatDiv.appendChild(detDiv);
                }

                chatDiv.setAttribute("data-id", d.Id);
                chatDiv.setAttribute("data-time", dateTime.format("YYYYMMDDHHmm"));
                
                t.chatConv.appendChild(chatDiv);
            }
        }
    }

    function extendDefaults(source, properties) {
        var property;
        for (property in properties) {
            if (properties.hasOwnProperty(property)) {
                source[property] = properties[property];
            }
        }
        return source;
    }

    function buildOut() {
        var content, contentHolder, docFrag;

        /*
         * If content is an HTML string, append the HTML string.
         * If content is a domNode, append its content.
         */

        if (typeof this.options.content === "string") {
            content = this.options.content;
        } else {
            content = this.options.content.innerHTML;
        }

        // Create a DocumentFragment to build with
        docFrag = document.createDocumentFragment();

        // Create modal element
        this.modal = document.createElement("div");
        this.modal.className = "sichat-box " + this.options.className;
        this.modal.style.minWidth = this.options.minWidth + "px";
        this.modal.style.maxWidth = this.options.maxWidth + "px";

        // If closeButton option is true, add a close button
        if (this.options.closeButton === true) {
            this.closeButton = document.createElement("a");
            this.closeButton.setAttribute("href", "javascript:;");
            this.closeButton.className = "sichat-close close-button";
            this.closeButton.innerHTML = "<i class='fa fa-times'></i>";
            this.modal.appendChild(this.closeButton);
        }

        // If overlay is true, add one
        if (this.options.overlay === true) {
            this.overlay = document.createElement("div");
            this.overlay.className = "sichat-overlay " + this.options.className;
            docFrag.appendChild(this.overlay);
        }

        // Create content area and append to modal
        contentHolder = document.createElement("div");
        contentHolder.className = "sichat-content";
        contentHolder.innerHTML = content;
        this.modal.appendChild(contentHolder);

        this.chatConv = document.createElement("div");
        this.chatConv.className = "sichat-conversation";
        this.modal.appendChild(this.chatConv);

        if (this.options.sendButton === true) {
            this.chatMessageContainer = document.createElement("div");
            this.chatMessageContainer.className = "sichat-chat-message-cont " + this.options.className;

            this.chatMessage = document.createElement("textarea");
            this.chatMessage.setAttribute("placeholder", this.options.chatPlaceHolder);
            this.chatMessage.className = "sichat-chat-message " + this.options.className;
            this.chatMessageContainer.appendChild(this.chatMessage);
            docFrag.appendChild(this.chatMessageContainer);
        } else {
            this.chatMessageContainer = document.createElement("div");
            this.chatMessageContainer.className = "sichat-chat-message-cont " + this.options.className;

            this.chatMessage = document.createElement("textarea");
            this.chatMessage.setAttribute("placeholder", this.options.chatPlaceHolder);
            this.chatMessage.className = "sichat-chat-message form-control " + this.options.className;
            this.chatMessageContainer.appendChild(this.chatMessage);
            this.modal.appendChild(this.chatMessageContainer);
        }

        // Append modal to DocumentFragment
        docFrag.appendChild(this.modal);

        // Append DocumentFragment to body
        document.body.appendChild(docFrag);

        this.toggleButton = document.querySelector("[data-toggle='chat']");
    }

    function initializeEvents() {
        if (this.closeButton) {
            this.closeButton.addEventListener('click', this.close.bind(this));
        }
        if (this.overlay) {
            this.overlay.addEventListener('click', this.close.bind(this));
        }
        if (!this.sendButton) {
            this.chatMessage.addEventListener("keydown", this.ignoreKeys.bind(this));
            this.chatMessage.addEventListener("keyup", this.sendMessage.bind(this));
        }
    }

    function transitionSelect() {
        var el = document.createElement("div");
        if (el.style.WebkitTransition) return "webkitTransitionEnd";
        if (el.style.OTransition) return "oTransitionEnd";
        return 'transitionend';
    }
}());
