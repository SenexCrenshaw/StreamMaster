import SMPopUp from '@components/sm/SMPopUp';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const About = () => {
  return (
    <SMPopUp showClose={false} title="About" info="" placement="bottom" icon="pi-question" buttonClassName="icon-yellow" contentWidthSize="2">
      <div className="flex flex-column sm-center-stuff">
        <img className="sm-w-2" alt="Stream Master Logo" src="/images/SMNewX32.png" />
        Stream Master
        <div className="flex  sm-center-stuff">
          <div className="flex flex-column w-full">
            <div className="layout-padding-bottom" />
            <div className="flex justify-content-start align-items-center">
              <img className="sm-w-2rem" alt="Stream Master Logo" src="/images/MrMonday.png" />- MrM0nday - UI
            </div>
            <div className="layout-padding-bottom" />
            <div className="flex ustify-content-start align-items-center">
              <img className="sm-w-2rem" alt="Stream Master Logo" src="/images/SenexBig.png" />- Senex Crenshaw - Dev
            </div>
            <div className="layout-padding-bottom" />
          </div>
        </div>
      </div>
    </SMPopUp>
  );
};

About.displayName = 'About';

export default memo(About);
