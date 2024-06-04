interface SMCardProperties {
  readonly center?: React.ReactNode;
  readonly children: React.ReactNode;
  readonly darkBackGround?: boolean;
  readonly header?: React.ReactNode;
  readonly title: string | undefined;
  readonly text?: string | undefined;
  readonly italicized?: boolean;
  readonly simple?: boolean;
}

export const SMCard = ({ center, children, darkBackGround = false, header, simple, title }: SMCardProperties) => {
  if (simple === true) {
    return <div>{children}</div>;
  }

  if (darkBackGround === true) {
    return (
      <div className="sm-card-dark">
        <div className="sm-card-header flex justify-content-between align-items-center">
          <div className="header-text-sub">{title}</div>
          <div className="pr-1">{header}</div>
        </div>
        <div className="layout-padding-bottom" />
        {children}
      </div>
    );
  }

  return (
    <div className="sm-card">
      <div className="sm-card-header flex justify-content-between align-items-center">
        <div className="header-text-sub">{title}</div>
        {center && center !== '' && <div className="px-1">{center}</div>}
        <div className="pr-1">{header}</div>
      </div>
      <div className="layout-padding-bottom" />
      {children}
    </div>
  );
};
