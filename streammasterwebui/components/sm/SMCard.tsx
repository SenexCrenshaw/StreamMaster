interface SMCardProperties {
  readonly children: React.ReactNode;
  readonly header?: React.ReactNode;
  readonly title: string | undefined;
  readonly text?: string | undefined;
  readonly italicized?: boolean;
}

export const SMCard = ({ children, header, italicized, text, title }: SMCardProperties) => {
  return (
    <div className="sm-card">
      <div className="sm-card-header flex justify-content-between align-items-center">
        <div className="header-text">{title}</div>
        <div className="pr-1">{header}</div>
      </div>
      <div className="layout-padding-bottom"></div>
      {children}
    </div>
  );
};
