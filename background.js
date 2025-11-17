let current = null; // {tabId, startAt, url, title}
const MIN_SESSION_SECONDS = 3; // minimum to push

chrome.tabs.onActivated.addListener(async activeInfo => {
  const tab = await chrome.tabs.get(activeInfo.tabId);
  handleSwitch(tab);
});

chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
  if (tab.active && changeInfo.status === 'complete') handleSwitch(tab);
});

chrome.windows.onFocusChanged.addListener(windowId => {
  if (windowId === chrome.windows.WINDOW_ID_NONE) stopCurrent();
});

function handleSwitch(tab) {
  // new active tab
  stopCurrent();
  // Only track http/https pages
  if (!tab.url || (!tab.url.startsWith('http://') && !tab.url.startsWith('https://'))) return;
  current = {
    tabId: tab.id,
    url: tab.url,
    title: tab.title,
    startAt: Date.now()
  };
}

function stopCurrent() {
  if (!current) return;
  const durationMs = Date.now() - current.startAt;
  const durationSec = Math.round(durationMs / 1000);
  if (durationSec >= MIN_SESSION_SECONDS) {
    const payload = {
      appName: new URL(current.url).hostname,
      startAt: new Date(current.startAt).toISOString(),
      durationSeconds: durationSec,
      attentionScore: 50, // default placeholder; could be improved
      contentType: null
    };

    // send to server: you'll want to replace the URL with your deployed server
    const serverUrl = 'https://your-server.example.com/api/Api/record';
    // If you're using an API key, store it in chrome.storage and append Authorization header
    chrome.storage.sync.get(['apiKey'], (res) => {
      const headers = {'Content-Type': 'application/json'};
      if (res.apiKey) headers['X-API-KEY'] = res.apiKey;
      fetch(serverUrl, {method: 'POST', headers, body: JSON.stringify(payload)})
        .catch(err => console.warn('post failed', err));
    });
  }
  current = null;
}

// also stop on idle messages (tab closed)
chrome.runtime.onMessage.addListener((msg, sender, sendResp) => {
  if (msg === 'stop') stopCurrent();
});