import AnimatedText from '@components/AnimatedText';
import { LinkButton } from '@components/buttons/LinkButton';
import SMPopUp from '@components/sm/SMPopUp';
import { useSMContext } from '@lib/signalr/SMProvider';
import { motion } from 'framer-motion';
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
      contentWidthSize="3"
    >
      <div className="flex flex-column sm-center-stuff">
        <div className="layout-padding-bottom" />
        <motion.img
          className="sm-w-4"
          alt="Stream Master Logo"
          src="/images/sm_logo.png"
          initial={{ scale: 1, rotate: 0, borderRadius: '0%' }}
          animate={{ scale: 1, rotate: 0, borderRadius: '0%' }}
          whileHover={{
            scale: [1, 2, 2, 1, 1],
            rotate: [0, 0, 180, 180, 0],
            borderRadius: ['0%', '0%', '50%', '50%', '0%'],
            transition: {
              duration: 2,
              ease: 'easeInOut',
              times: [0, 0.2, 0.5, 0.8, 1],
              repeat: Infinity,
              repeatDSelay: 1
            }
          }}
        />
        Stream Master
        <div className="col-6 m-0 p-0 justify-content-center align-content-start text-xs text-center">
          <div className="sm-text-xs">
            <LinkButton justText title={settings.Release ?? ''} link={settings.Release ?? ''} />
          </div>
        </div>
        <div className="flex sm-center-stuff">
          <div className="flex flex-column w-full sm-center-stuff">
            <div className="layout-padding-bottom" />
            <div className="flex justify-content-center align-items-center">
              <motion.img
                className="sm-w-3rem"
                alt="Senex Crenshaw"
                src="/images/mrmonday_logo_sm.png"
                initial={{ scale: 1, rotate: 0, borderRadius: '0%' }}
                animate={{ scale: 1, rotate: 0, borderRadius: '0%' }}
                whileHover={{
                  scale: [1, 2, 2, 1, 1],
                  rotate: [0, 0, 180, 180, 0],
                  borderRadius: ['0%', '0%', '50%', '50%', '0%'],
                  transition: {
                    duration: 2,
                    ease: 'easeInOut',
                    times: [0, 0.2, 0.5, 0.8, 1],
                    repeat: Infinity,
                    repeatDSelay: 1
                  }
                }}
              />
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <AnimatedText text="- MrM0nday -" fontSize={18} color="var(--text-color)" />
                <AnimatedText text="UI" fontSize={18} color="var(--text-color)" />
              </div>
            </div>
            <div className="layout-padding-bottom" />
            <div className="flex justify-content-center align-items-center">
              <motion.img
                className="sm-w-3rem"
                alt="Senex Crenshaw"
                src="/images/senex_logo_sm.png"
                initial={{ scale: 1, rotate: 0, borderRadius: '0%' }}
                animate={{ scale: 1, rotate: 0, borderRadius: '0%' }}
                whileHover={{
                  scale: [1, 2, 2, 1, 1],
                  rotate: [0, 0, 180, 180, 0],
                  borderRadius: ['0%', '0%', '50%', '50%', '0%'],
                  transition: {
                    duration: 2,
                    ease: 'easeInOut',
                    times: [0, 0.2, 0.5, 0.8, 1],
                    repeat: Infinity,
                    repeatDSelay: 1
                  }
                }}
              />
              <div className="pl-1 text-lg flex flex-column sm-center-stuff">
                <AnimatedText text="- Senex Crenshaw -" fontSize={18} color="var(--text-color)" />
                <AnimatedText text="Dev" fontSize={18} color="var(--text-color)" />
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
