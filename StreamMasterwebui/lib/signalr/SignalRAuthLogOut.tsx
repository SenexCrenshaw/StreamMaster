export const authLogOut = (): void => {
  const currentUrl = window.location.pathname + window.location.search + window.location.hash;
  const returnUrl = encodeURIComponent(currentUrl);
  window.location.href = `/login?ReturnUrl=${returnUrl}`;
};
