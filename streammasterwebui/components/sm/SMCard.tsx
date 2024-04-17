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
      <div className="sm-card-header flex justify-content-between align-items-center px-1">
        <span className="header-text-color">{title}</span>
        {header}
      </div>
      {children}
    </div>
  );
};
