interface SMCardProperties {
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly title: string | undefined;
  readonly text?: string | undefined;
  readonly italicized?: boolean;
  readonly simple?: boolean;
}

export const SMCard = ({ children, header, italicized, simple, text, title }: SMCardProperties) => {
  if (simple === true) {
    return <div>{children}</div>;
  }

  return (
    <div className="sm-card">
      <div className="sm-card-header flex justify-content-between align-items-center">
        <div className="header-text-sub">{title}</div>
        <div className="pr-1">{header}</div>
      </div>
      <div className="layout-padding-bottom"></div>
      {children}
    </div>
  );
};
