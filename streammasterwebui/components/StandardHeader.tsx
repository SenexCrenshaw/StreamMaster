interface StandardHeaderProperties {
  readonly className?: string;
  readonly displayName: string;
  readonly icon: JSX.Element;
  readonly children: React.ReactNode;
}
const StandardHeader = ({ children, className, displayName, icon }: StandardHeaderProperties) => (
  <div className={`${className} h-full`}>
    <div className="grid grid-nogutter flex justify-content-between align-items-center">
      <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
        <span className="ml-1">{icon}</span>
        <span className="ml-2">{displayName.toUpperCase()}</span>
      </div>
      <div className="flex col-12 mt-1 m-0 p-0">
        <div className={`${className} flex w-full min-w-full col-12`}>{children}</div>
      </div>
    </div>
  </div>
);

export default StandardHeader;
