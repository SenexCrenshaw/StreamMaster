import { LinkButton } from '@components/buttons/LinkButton';
import { Direction, Shape } from '@components/sm/Interfaces/SMSpeedDialTypes';
import SMPopUp from '@components/sm/SMPopUp';
import SMSpeedMenu from '@components/sm/SMSpeedMenu';
import { useSMContext } from '@lib/context/SMProvider';
import { motion } from 'framer-motion';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const SpeedMenuTest = () => {
  const { settings } = useSMContext();

  const mainMrM0nday = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'left' as Direction,
    icon: '/images/mrmonday_logo_sm.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainSenex = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'right' as Direction,
    icon: '/images/senex_logo_sm.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainItemCircle = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'circle' as Shape
  };
  const mainItemLineTop = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'top' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'line' as Shape
  };

  const mainItemLineRight = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'right' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'line' as Shape
  };

  const mainItemLineBottom = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'bottom' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'line' as Shape
  };

  const mainItemLineLeft = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'left' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'line' as Shape
  };

  const mainItemSemiTop = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'top' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainItemSemiRight = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'right' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainItemSemiBottom = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'bottom' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainItemSemiLeft = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'left' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'semicircle' as Shape
  };

  const mainItemQtrTop = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'top' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrRight = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'right' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrBottom = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'bottom' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrLeft = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'left' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrTopLeft = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'top-left' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrTopRight = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'top-right' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrBottomLeft = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'bottom-left' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const mainItemQtrBottomRight = {
    animateOn: 'hover' as 'hover',
    command: () => console.log('Main Icon clicked'),
    direction: 'bottom-right' as Direction,
    icon: '/images/sm_logo.png',
    modal: true,
    shape: 'quarter-circle' as Shape
  };

  const items = [
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
        <motion.img
          className="sm-w-4"
          alt="Stream Master Logo"
          src="/images/sm_logo.png"
          initial={{ borderRadius: '0%', rotate: 0, scale: 1 }}
          animate={{ borderRadius: '0%', rotate: 0, scale: 1 }}
          whileHover={{
            borderRadius: ['0%', '0%', '50%', '50%', '0%'],
            // rotate: [0, 0, 180, 180, 0],
            scale: [1, 2, 2, 1, 1],
            transition: {
              duration: 2,
              ease: 'easeInOut',
              repeat: Infinity,
              repeatDSelay: 1,
              times: [0, 0.2, 0.5, 0.8, 1]
            }
          }}
        />
        Stream Master
        <div className="col-6 m-0 p-0 justify-content-center align-content-start text-xs text-center">
          <div className="sm-text-xs sm-center-stuff w-full">
            <LinkButton justText title={settings.Release ?? ''} link={settings.Release ?? ''} />
          </div>
        </div>
        <div className="flex sm-center-stuff">
          <div className="flex flex-column w-full sm-center-stuff">
            <div className="layout-padding-bottom" />
            {/* <div className="flex justify-content-center align-items-center">
              <motion.img
                className="sm-w-3rem"
                alt="Senex Crenshaw"
                src="/images/mrmonday_logo_sm.png"
                initial={{ borderRadius: '0%', rotate: 0, scale: 1 }}
                animate={{ borderRadius: '0%', rotate: 0, scale: 1 }}
                whileHover={{
                  borderRadius: ['0%', '0%', '50%', '50%', '0%'],
                  // rotate: [0, 0, 180, 180, 0],
                  scale: [1, 2, 2, 1, 1],
                  transition: {
                    duration: 2,
                    ease: 'easeInOut',
                    repeat: Infinity,
                    repeatDSelay: 1,
                    times: [0, 0.2, 0.5, 0.8, 1]
                  }
                }}
              />
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <AnimatedText text="- MrM0nday -" fontSize={18} color="var(--text-color)" />
                <div className="font-italic text-xs" color="var(--text-color)">
                  UI
                </div>
              </div>
            </div>
            <div className="layout-padding-bottom" /> */}
            <div className="flex justify-content-center align-items-center">
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <SMSpeedMenu items={items} mainItem={mainMrM0nday} />
                <div className="font-italic text-xs" color="var(--text-color)">
                  UI
                </div>
              </div>
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <SMSpeedMenu items={items} mainItem={mainSenex} />
                <div className="font-italic text-xs" color="var(--text-color)">
                  Dev
                </div>
              </div>
              {/* <div className="flex flex-column">
                <div className="flex w-12">
                  <SMSpeedMenu items={items} mainItem={mainItemLineTop} />
                  <SMSpeedMenu items={items} mainItem={mainItemLineRight} />

                  <SMSpeedMenu items={items} mainItem={mainItemLineBottom} />
                  <SMSpeedMenu items={items} mainItem={mainItemLineLeft} />
                </div>
                <div className="flex w-12">
                  <SMSpeedMenu items={items} mainItem={mainItemSemiTop} />
                  <SMSpeedMenu items={items} mainItem={mainItemSemiRight} />

                  <SMSpeedMenu items={items} mainItem={mainItemSemiBottom} />
                  <SMSpeedMenu items={items} mainItem={mainItemSemiLeft} />
                </div>
                <div className="flex w-12">
                  <SMSpeedMenu items={items} mainItem={mainItemCircle} />
                </div>
                <div className="flex w-12">
                  <SMSpeedMenu items={items} mainItem={mainItemQtrTopLeft} />
                  <SMSpeedMenu items={items} mainItem={mainItemQtrTop} />
                  <SMSpeedMenu items={items} mainItem={mainItemQtrTopRight} />

                  <SMSpeedMenu items={items} mainItem={mainItemQtrRight} />
                </div>
                <div className="flex w-12">
                  <SMSpeedMenu items={items} mainItem={mainItemQtrBottomLeft} />
                  <SMSpeedMenu items={items} mainItem={mainItemQtrBottom} />

                  <SMSpeedMenu items={items} mainItem={mainItemQtrBottomRight} />
                  <SMSpeedMenu items={items} mainItem={mainItemQtrLeft} />
                </div>
              </div> */}
              {/* <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <AnimatedText text="- Senex Crenshaw -" fontSize={18} color="var(--text-color)" />
                <div className="font-italic text-xs" color="var(--text-color)">
                  Dev
                </div>
              </div> */}
            </div>

            <div className="layout-padding-bottom" />
          </div>
        </div>
      </div>
    </SMPopUp>
  );
};

SpeedMenuTest.displayName = 'SpeedMenuTest';

export default memo(SpeedMenuTest);
