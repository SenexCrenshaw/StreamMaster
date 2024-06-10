import { useMemo } from 'react';
import { SMCardProperties } from './Interfaces/SMCardProperties';

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
      <div className="sm-card-header flex justify-content-between align-items-center">
        <div className="header-text-sub">{title}</div>
        <div className="pr-1">{header}</div>
      </div>
      <div className="layout-padding-bottom" />
      {children}
    </div>
  );

  // if (darkBackGround === true) {
  //   return (
  //     <div className="sm-card-dark">
  //       <div className="sm-card-header flex justify-content-between align-items-center">
  //         <div className="header-text-sub">{title}</div>
  //         <div className="pr-1">{header}</div>
  //       </div>
  //       <div className="layout-padding-bottom" />
  //       {children}
  //     </div>
  //   );
  // }

  // return (
  //   <div className="sm-card">
  //     <div className="sm-card-header flex justify-content-between align-items-center">
  //       <div className="header-text-sub">{title}</div>
  //       {center && center !== '' && <div className="px-1">{center}</div>}
  //       <div className="pr-1">{header}</div>
  //     </div>
  //     <div className="layout-padding-bottom" />
  //     {children}
  //   </div>
  // );
};
