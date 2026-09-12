"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// Elements
const sendButton = document.getElementById("sendButton");
const messageInput = document.getElementById("messageInput");
const userInput = document.getElementById("userInput");
const messagesList = document.getElementById("messagesList");
const statusBadge = document.getElementById("statusBadge");
const emptyState = document.getElementById("emptyState");

// Disable send button until connection is established
sendButton.disabled = true;

// Predefined palette for nice avatar gradient circles
const avatarGradients = [
    "linear-gradient(135deg, #6366f1, #8b5cf6)",
    "linear-gradient(135deg, #06b6d4, #3b82f6)",
    "linear-gradient(135deg, #10b981, #059669)",
    "linear-gradient(135deg, #f59e0b, #d97706)",
    "linear-gradient(135deg, #ec4899, #be185d)",
    "linear-gradient(135deg, #8b5cf6, #ec4899)",
    "linear-gradient(135deg, #14b8a6, #0d9488)"
];

function getAvatarBackground(name) {
    if (!name) return avatarGradients[0];
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
        hash = name.charCodeAt(i) + ((hash << 5) - hash);
    }
    const index = Math.abs(hash) % avatarGradients.length;
    return avatarGradients[index];
}

// Receive message handler from SignalR Hub
connection.on("ClientRecieveMessageEvent", function (user, message) {
    if (emptyState) {
        emptyState.style.display = "none";
    }

    const timeString = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    const initial = user && user.trim().length > 0 ? user.trim().charAt(0).toUpperCase() : "?";
    const bg = getAvatarBackground(user);

    // Create message element with:
    // 1. Default Avatar circle
    // 2. Beside: Name + Timestamp
    // 3. Under: Message bubble
    const messageItem = document.createElement("div");
    messageItem.className = "chat-message-item";

    messageItem.innerHTML = `
        <div class="chat-avatar" style="background: ${bg};" title="${escapeHtml(user)}">${escapeHtml(initial)}</div>
        <div class="chat-content">
            <div class="chat-author-row">
                <span class="chat-author-name">${escapeHtml(user)}</span>
                <span class="chat-timestamp">${escapeHtml(timeString)}</span>
            </div>
            <div class="chat-text-bubble">${escapeHtml(message)}</div>
        </div>
    `;

    messagesList.appendChild(messageItem);
    messagesList.scrollTop = messagesList.scrollHeight;
});

// Start the connection
connection.start().then(function () {
    sendButton.disabled = false;
    if (statusBadge) {
        statusBadge.className = "status-badge";
        statusBadge.innerHTML = '<span class="status-dot"></span> <span>Connected</span>';
    }
}).catch(function (err) {
    if (statusBadge) {
        statusBadge.className = "status-badge disconnected";
        statusBadge.innerHTML = '<span class="status-dot"></span> <span>Disconnected</span>';
    }
    return console.error(err.toString());
});

// Send message function
function sendMessage() {
    const user = userInput.value.trim();
    const message = messageInput.value.trim();

    if (!user) {
        userInput.focus();
        return;
    }
    if (!message) {
        messageInput.focus();
        return;
    }

    connection.invoke("ServerRecieveMessageEvent", user, message).catch(function (err) {
        return console.error(err.toString());
    });

    messageInput.value = "";
    messageInput.focus();
}

// Send on button click
sendButton.addEventListener("click", function (event) {
    sendMessage();
    event.preventDefault();
});

// Send on Enter key press
messageInput.addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        sendMessage();
        event.preventDefault();
    }
});

// Helper function to safely escape HTML
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
