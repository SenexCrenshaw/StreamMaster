import React from "react";
import {
  TimelineWrapper,
  TimelineBox,
  TimelineTime,
  TimelineDivider,
  TimelineDividers,
  useTimeline,
} from "planby";

type TimelineProps = {
  readonly dayWidth: number;
  readonly hourWidth: number;
  readonly isBaseTimeFormat: boolean;
  readonly isSidebar: boolean;
  readonly numberOfHoursInDay: number;
  readonly offsetStartHoursRange: number;
  readonly sidebarWidth: number;
}

const Timeline = ({
  isBaseTimeFormat,
  isSidebar,
  dayWidth,
  hourWidth,
  numberOfHoursInDay,
  offsetStartHoursRange,
  sidebarWidth,
}: TimelineProps) => {
  const { time, dividers, formatTime } = useTimeline(
    numberOfHoursInDay,
    isBaseTimeFormat
  );

  const renderDividers = () =>
    dividers.map((_, index) => (

      // eslint-disable-next-line react/no-array-index-key
      <TimelineDivider key={index} width={hourWidth} />
    ));

  const renderTime = (index: number) => (
    <TimelineBox key={index} width={hourWidth}>
      <TimelineTime>
        {formatTime(index + offsetStartHoursRange).toLowerCase()}
      </TimelineTime>
      <TimelineDividers>{renderDividers()}</TimelineDividers>
    </TimelineBox>
  );


  return (
    <TimelineWrapper
      dayWidth={dayWidth}
      isSidebar={isSidebar}
      sidebarWidth={sidebarWidth}
    >
      {time.map((_, index) => renderTime(index))}
    </TimelineWrapper>
  );
}

export default React.memo(Timeline);
