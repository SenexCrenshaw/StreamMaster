import { useMemo } from 'react';
import { SMCardProperties } from './interfaces/SMCardProperties';

interface InternalSMCardProperties extends SMCardProperties {
  readonly children: React.ReactNode;
}

export const SMCard = ({ center, children, darkBackGround = false, header, simple, title }: InternalSMCardProperties) => {
  const getDiv = useMemo(() => {
    return darkBackGround === true ? 'sm-card-dark' : 'sm-card';
  }, [darkBackGround]);

  if (simple === true || title === undefined || title === '') {
    return <div>{children}</div>;
  }

  return (
    <div className={getDiv}>
      <div className="sm-card-header flex flex-row justify-content-between align-items-center">
        <div className="header-text-sub">{title}</div>
        <div className="pr-1 flex gap-1">{header}</div>
      </div>
      <div className="layout-padding-bottom" />
      {children}
    </div>
  );
};
