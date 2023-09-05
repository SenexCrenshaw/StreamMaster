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
  dayWidth: number;
  hourWidth: number;
  isBaseTimeFormat: boolean;
  isSidebar: boolean;
  numberOfHoursInDay: number;
  offsetStartHoursRange: number;
  sidebarWidth: number;
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
