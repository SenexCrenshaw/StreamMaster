export const authLogOut = (): void => {
  const currentUrl = window.location.pathname + window.location.search + window.location.hash;
  const returnUrl = encodeURIComponent(currentUrl);
  const loginUrl = `/login?ReturnUrl=${returnUrl}`;

  // Prevent redirecting if the current URL already matches the intended login URL
  if (window.location.href !== window.location.origin + loginUrl) {
    window.location.href = loginUrl;
  }
};
