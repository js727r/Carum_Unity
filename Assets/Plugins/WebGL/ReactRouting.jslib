mergeInto(LibraryManager.library, {
  ReactRouting: function (to) {
    try {
      window.dispatchReactUnityEvent("ReactRouting",
      Pointer_stringify(to)
      );
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  }
});