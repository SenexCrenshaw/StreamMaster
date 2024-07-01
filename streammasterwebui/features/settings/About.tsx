import { LinkButton } from '@components/buttons/LinkButton';
import { Direction, Shape } from '@components/sm/Interfaces/SMSpeedDialTypes';
import SMPopUp from '@components/sm/SMPopUp';
import SMSpeedMenu from '@components/sm/SMSpeedMenu';
import { Logger } from '@lib/common/logger';
import { useSMContext } from '@lib/signalr/SMProvider';
import { motion } from 'framer-motion';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const About = () => {
  const { settings } = useSMContext();

  const mainMrM0nday = {
    animateOn: 'hover' as 'hover',
    direction: 'left' as Direction,
    icon: '/images/mrmonday_logo_sm.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainSenex = {
    animateOn: 'hover' as 'hover',
    direction: 'right' as Direction,
    icon: '/images/senex_logo_sm.png',
    modal: true,
    shape: 'line' as Shape
  };

  const mainSM = {
    animateOn: 'hover' as 'hover',
    direction: 'top' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'circle' as Shape
  };

  const smItems = [
    {
      animateOn: 'hover' as 'hover',
      icon: 'smspeed-menu-icon pi pi-github',
      url: 'https://github.com/SenexCrenshaw/StreamMaster'
    },
    {
      animateOn: 'hover' as 'hover',
      icon: 'smspeed-menu-icon icon-red pi pi-heart-fill',
      url: 'https://github.com/sponsors/SenexCrenshaw'
    }
  ];

  const senexItems = [
    {
      animateOn: 'hover' as 'hover',
      icon: 'smspeed-menu-icon pi pi-github',
      url: 'https://github.com/SenexCrenshaw/StreamMaster'
    },
    {
      animateOn: 'hover' as 'hover',
      icon: 'smspeed-menu-icon icon-red pi pi-heart-fill',
      url: 'https://github.com/sponsors/SenexCrenshaw'
    }
  ];

  const mrm0pndayItems = [
    {
      animateOn: 'hover' as 'hover',
      command: () => alert('Icon 1 clicked'),
      icon: '/images/sm_logo.png'
    },
    {
      animateOn: 'hover' as 'hover',
      command: () => alert('Icon 2 clicked'),
      icon: '/images/mrmonday_logo_sm.png'
    },
    { animateOn: 'hover' as 'hover', command: () => alert('Icon 2 clicked'), icon: '/images/senex_logo_sm.png' },
    {
      animateOn: 'hover' as 'hover',
      command: () => alert('Icon 2 clicked'),
      icon: '/images/mrmonday_logo_sm.png'
    }
    // Add more items as needed
  ];

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
      contentWidthSize="3"
    >
      <div className="flex flex-column sm-center-stuff">
        <div className="layout-padding-bottom" />
        <SMSpeedMenu items={smItems} mainItem={mainSM} />
        Stream Master
        <div className="col-6 m-0 p-0 justify-content-center align-content-start text-xs text-center">
          <div className="sm-text-xs sm-center-stuff w-full">
            <LinkButton justText title={settings.Release ?? ''} link={settings.Release ?? ''} />
          </div>
        </div>
        <div className="flex sm-center-stuff">
          <div className="flex flex-column w-full sm-center-stuff">
            <div className="layout-padding-bottom" />

            <div className="flex justify-content-center align-items-center">
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <SMSpeedMenu items={mrm0pndayItems} mainItem={mainMrM0nday} />
                <div className="font-italic text-xs" color="var(--text-color)">
                  UI
                </div>
              </div>
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <SMSpeedMenu items={senexItems} mainItem={mainSenex} />
                <div className="font-italic text-xs" color="var(--text-color)">
                  Dev
                </div>
              </div>
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
