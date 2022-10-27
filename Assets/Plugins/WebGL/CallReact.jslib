mergeInto(LibraryManager.library, {
  CallReact: function (userName,score) {
    try {
      window.dispatchReactUnityEvent("CallReact",
      Pointer_stringify(userName),
      score);
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  }
});