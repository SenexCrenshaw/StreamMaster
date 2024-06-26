import { LinkButton } from '@components/buttons/LinkButton';
import SMPopUp from '@components/sm/SMPopUp';
import { useSMContext } from '@lib/signalr/SMProvider';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const About = () => {
  const { settings } = useSMContext();
  return (
    <SMPopUp
      modal
      modalCentered
      showClose={false}
      title="About"
      info=""
      placement="bottom"
      icon="pi-question"
      buttonClassName="icon-yellow"
      contentWidthSize="2"
    >
      <div className="flex flex-column sm-center-stuff">
        <div className="layout-padding-bottom" />
        <img className="sm-w-2" alt="Stream Master Logo" src="/images/SMNewX32.png" />
        Stream Master
        <div className="col-6 m-0 p-0 justify-content-center align-content-start text-xs text-center">
          <div className="sm-text-xs sm-w-8rem">
            <LinkButton justText title={settings.Release ?? ''} link={settings.Release ?? ''} />
          </div>
        </div>
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
