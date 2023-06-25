import React from "react";
import {
  type ProgramItem
} from "planby";

import {
  ProgramBox,
  ProgramContent,
  ProgramFlex,
  ProgramStack,
  ProgramTitle,
  ProgramText,
  ProgramImage,
  useProgram,
} from "planby";

const ProgramComponent = ({ program, onClick, ...rest }: ProgramComponentProps) => {
  const { styles, formatTime, set12HoursTimeFormat, isLive, isMinWidth } =
    useProgram({
      program,
      ...rest,
    });

  const { data } = program;
  const { description, image, title, since, till } = data;

  const sinceTime = formatTime(since, set12HoursTimeFormat()).toLowerCase();
  const tillTime = formatTime(till, set12HoursTimeFormat()).toLowerCase();

  return (
    <ProgramBox onClick={() => {
      onClick(program.data.videoStreamId)
    }} style={styles.position} width={styles.width}>
      <ProgramContent isLive={isLive} width={styles.width}>
        <ProgramFlex>
          {isLive && isMinWidth && <ProgramImage alt="Preview" src={image} />}
          <ProgramStack>
            <ProgramTitle>{title}</ProgramTitle>
            <ProgramText>
              {description}<br />
              {sinceTime} - {tillTime}
            </ProgramText>
          </ProgramStack>
        </ProgramFlex>
      </ProgramContent>
    </ProgramBox>
  );
};


type ProgramProps = {

  onClick: ((videoStreamId: number) => void);

};

type ProgramComponentProps = ProgramItem & ProgramProps;

export default React.memo(ProgramComponent);
