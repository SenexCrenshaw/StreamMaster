import About from '@features/settings/About';

interface StandardHeaderProperties {
  readonly className?: string;
  readonly displayName: string | JSX.Element;
  readonly icon: JSX.Element;
  readonly children: React.ReactNode;
}
const StandardHeader = ({ children, className, displayName, icon }: StandardHeaderProperties) => (
  <div className={`${className}`}>
    <div className="sm-standard-header flex flex-row justify-content-between align-items-center">
      <div className="flex flex-row font-bold justify-content-start align-items-center">
        <span className="flex ml-1">{icon}</span>
        <span className="flex ml-2">{typeof displayName === 'string' ? displayName.toUpperCase() : displayName}</span>
      </div>
      <div className="pr-1">
        <About />
      </div>
    </div>
    <div className="flex layout-padding-bottom"></div>
    <div className="flex flex-row w-12">{children}</div>
  </div>
);

export default StandardHeader;
